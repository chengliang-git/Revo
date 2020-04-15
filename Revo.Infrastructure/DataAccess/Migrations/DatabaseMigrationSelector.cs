﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;

namespace Revo.Infrastructure.DataAccess.Migrations
{
    public class DatabaseMigrationSelector : IDatabaseMigrationSelector
    {
        private readonly IDatabaseMigrationRegistry migrationRegistry;
        private readonly IDatabaseMigrationProvider migrationProvider;
        private IReadOnlyCollection<IDatabaseMigrationRecord> history = null;

        public DatabaseMigrationSelector(IDatabaseMigrationRegistry migrationRegistry, IDatabaseMigrationProvider migrationProvider)
        {
            this.migrationRegistry = migrationRegistry;
            this.migrationProvider = migrationProvider;
        }
        
        public async Task<IReadOnlyCollection<SelectedModuleMigrations>> SelectMigrationsAsync(DatabaseMigrationSpecifier[] modules, string[] tags)
        {
            try
            {
                var queuedMigrations = new List<DatabaseMigrationSpecifier>();
                var result = new List<SelectedModuleMigrations>();

                foreach (var module in modules)
                {
                    var migrations = await DoSelectMigrationsAsync(module.ModuleName, tags, module.Version, queuedMigrations);

                    if (migrations.Count > 0)
                    {
                        result.Add(new SelectedModuleMigrations(module, migrations));
                    }
                }

                return result;
            }
            finally
            {
                history = null;
            }
        }

        private async Task<IReadOnlyCollection<IDatabaseMigration>> DoSelectMigrationsAsync(string moduleName, string[] tags,
            DatabaseVersion targetVersion, List<DatabaseMigrationSpecifier> queuedMigrations)
        {
            IEnumerable<IDatabaseMigration> moduleMigrations = migrationRegistry.Migrations
                .Where(x => string.Equals(x.ModuleName, moduleName, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => x.Tags.All(tagGroup => tags.Any(tagGroup.Contains)));

            if (targetVersion != null)
            {
                moduleMigrations = moduleMigrations.Where(x => x.Version.CompareTo(targetVersion) <= 0);
            }

            moduleMigrations = moduleMigrations
                .OrderBy(x => x.Version)
                .ToArray();

            if (targetVersion != null)
            {
                if (moduleMigrations.Any() && moduleMigrations.Last().IsRepeatable)
                {
                    throw new DatabaseMigrationException($"Cannot select database migrations for module '{moduleName}' version {targetVersion} because it is a repeatable migration module, which means it is versioned only by checksums");
                }
            }
            else if (!moduleMigrations.Any())
            {
                // TODO maybe return without errors if there are migrations for this module with different tags?
                throw new DatabaseMigrationException($"Cannot select database migrations for module '{moduleName}': no migrations for specified module were found");
            }
            
            List<IDatabaseMigration> result = new List<IDatabaseMigration>();
            
            if (moduleMigrations.Any() && moduleMigrations.Last().IsRepeatable) // repeatable-migration module
            {
                if (!queuedMigrations.Any(x => string.Equals(x.ModuleName, moduleName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    history = history ?? await migrationProvider.GetMigrationHistoryAsync();
                    string lastMigrationChecksum = history
                        .Where(x => string.Equals(x.ModuleName, moduleName, StringComparison.InvariantCultureIgnoreCase))
                        .OrderByDescending(x => x.TimeApplied)
                        .Select(x => x.Checksum).FirstOrDefault();

                    var migration = moduleMigrations.Last();
                    if (lastMigrationChecksum != migration.Checksum)
                    {
                        result.Add(migration);
                    }
                }
            }
            else
            {
                var version = queuedMigrations
                    .Where(x => string.Equals(x.ModuleName, moduleName, StringComparison.InvariantCultureIgnoreCase))
                    .OrderByDescending(x => x.Version)
                    .Select(x => x.Version)
                    .FirstOrDefault();

                if (version == null)
                {
                    history = history ?? await migrationProvider.GetMigrationHistoryAsync();
                    var lastMigration = history
                        .Where(x => string.Equals(x.ModuleName, moduleName, StringComparison.InvariantCultureIgnoreCase))
                        .OrderByDescending(x => x.Version).FirstOrDefault();
                    version = lastMigration?.Version;
                }
                
                if (version == null)
                {
                    var baseline = moduleMigrations.FirstOrDefault(x => x.IsBaseline);
                    if (baseline != null)
                    {
                        result.Add(baseline);
                        result.AddRange(moduleMigrations
                            .Where(x => x.Version.CompareTo(baseline.Version) > 0));
                    }
                    else
                    {
                        result.AddRange(moduleMigrations);
                    }
                }
                else
                {
                    result.AddRange(moduleMigrations
                        .Where(x => !x.IsBaseline && x.Version.CompareTo(version) > 0));
                }
                
                if (targetVersion != null)
                {
                    if ((version == null || version.CompareTo(targetVersion) < 0)
                        && (!result.Any() || !Equals(result.LastOrDefault()?.Version, targetVersion)))
                    {
                        throw new DatabaseMigrationException($"Cannot select database migrations for module '{moduleName}': migration for required version {targetVersion} from {version?.ToString() ?? "nothing"} was not found");
                    }
                }
                else
                {
                    var maxVersion = moduleMigrations.Select(x => x.Version).Max();

                    if ((!result.Any() && (version == null || version.CompareTo(maxVersion) < 0))
                        || (result.Any() && !result.Last().Version.Equals(maxVersion)))
                    {
                        throw new DatabaseMigrationException($"Cannot select database migrations for module '{moduleName}': migration path to required version 'latest' ({maxVersion}) from {version?.ToString() ?? "nothing"} was not found");
                    }
                }
            }

            foreach (var migration in result)
            {
                queuedMigrations.Add(new DatabaseMigrationSpecifier(migration.ModuleName, migration.Version));
            }

            await AddRequiredDependenciesAsync(result, queuedMigrations, tags);
            return result;
        }

        private async Task AddRequiredDependenciesAsync(List<IDatabaseMigration> migrations,
            List<DatabaseMigrationSpecifier> queuedMigrations, string[] tags)
        {
            for (int i = 0; i < migrations.Count; i++)
            {
                var migration = migrations[i];

                var dependencySpecs = await GetRequiredMigrationDependenciesAsync(migration, queuedMigrations, tags);
                foreach (var dependencySpec in dependencySpecs)
                {
                    var dependencyMigrations = await DoSelectMigrationsAsync(dependencySpec.ModuleName, tags,
                        dependencySpec.Version, queuedMigrations);
                    migrations.InsertRange(i, dependencyMigrations);
                    queuedMigrations.AddRange(dependencyMigrations.Select(x => new DatabaseMigrationSpecifier(x.ModuleName, x.Version)));
                    i += dependencyMigrations.Count;
                }
            }
        }

        private async Task<List<DatabaseMigrationSpecifier>> GetRequiredMigrationDependenciesAsync(
            IDatabaseMigration migration, List<DatabaseMigrationSpecifier> queuedMigrations, string[] tags)
        {
            var dependencies = new List<DatabaseMigrationSpecifier>();

            history = history ?? await migrationProvider.GetMigrationHistoryAsync();

            var tagMatchingMigrations = migrationRegistry.Migrations
                .Where(x => x.Tags.All(tagGroup => tags.Any(tagGroup.Contains)));

            foreach (var dependency in migration.Dependencies)
            {
                // repeatable dependencies
                var repeatableMigration = tagMatchingMigrations.FirstOrDefault(x => x.ModuleName == dependency.ModuleName && x.IsRepeatable);
                if (dependency.Version == null && repeatableMigration != null)
                {
                    if (queuedMigrations.Any(x => x.ModuleName == dependency.ModuleName))
                    {
                        continue;
                    }

                    var lastMigration = history
                        .Where(x => string.Equals(x.ModuleName, dependency.ModuleName, StringComparison.InvariantCultureIgnoreCase))
                        .OrderByDescending(x => x.TimeApplied).FirstOrDefault();

                    if (lastMigration?.Checksum == repeatableMigration.Checksum)
                    {
                        continue;
                    }

                    dependencies.Add(dependency);
                }
                // versioned dependencies
                else
                {
                    var version = queuedMigrations
                        .Where(x => string.Equals(x.ModuleName, dependency.ModuleName, StringComparison.InvariantCultureIgnoreCase))
                        .OrderByDescending(x => x.Version)
                        .Select(x => x.Version)
                        .FirstOrDefault();

                    if (version == null)
                    {
                        var lastMigration = history
                            .Where(x => string.Equals(x.ModuleName, dependency.ModuleName, StringComparison.InvariantCultureIgnoreCase))
                            .OrderByDescending(x => x.Version).FirstOrDefault();
                        version = lastMigration?.Version;
                    }

                    IEnumerable<IDatabaseMigration> matchingDependencies = tagMatchingMigrations
                        .Where(x => x.ModuleName == dependency.ModuleName)
                        .OrderByDescending(x => x.Version);

                    if (dependency.Version != null)
                    {
                        if (version != null && version.CompareTo(dependency.Version) >= 0)
                        {
                            continue; // skip if nothing to upgrade, an actual migration to that version does not need to exist
                        }

                        matchingDependencies = matchingDependencies.Where(x => dependency.Version.Equals(x.Version)); // case when !matchingDependencies.Any() is handled later
                    }
                    else
                    {
                        if (matchingDependencies.Any())
                        {
                            var maxVersion = matchingDependencies.Select(x => x.Version).Max();
                            if (version != null && version.CompareTo(maxVersion) >= 0)
                            {
                                continue; // nothing to upgrade, case when !matchingDependencies.Any() is handled later
                            }

                            matchingDependencies = matchingDependencies.Where(x => maxVersion.Equals(x.Version));
                        }
                    }
                    
                    if (!matchingDependencies.Any())
                    {
                        throw new DatabaseMigrationException($"Database migration {migration} specifies dependency to {dependency.ModuleName}@{dependency.Version?.ToString() ?? "latest"}, but no matching migrations to that version were found");
                    }
                    
                    matchingDependencies = matchingDependencies.ToArray();

                    if (version != null)
                    {
                        matchingDependencies = matchingDependencies.Where(x => !x.IsBaseline);

                        if (!matchingDependencies.Any())
                        {
                            throw new DatabaseMigrationException($"Database migration {migration} specifies dependency to {dependency.ModuleName}@{dependency.Version?.ToString() ?? "latest"}, for which there is a baseline migration, but no upgrade migration from version {version}");
                        }
                    }

                    dependencies.Add(dependency);
                }
                
            }

            return dependencies;
        }
    }
}