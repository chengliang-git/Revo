/*
ModuleId: 5F08DF74-3C4B-4DBD-AD35-26565239E157
Version: 1.0.0
ScriptId: CDED157F-2538-4F33-81E0-4B2DD1BCC4AA
*/

IF	NOT EXISTS (SELECT 1 FROM [REV_SCRIPT_LOG] WHERE [REV_SCL_ScriptId] = '1FBA6358-7AC1-4B6F-82D6-48BBCBD5C99E' AND [REV_SCL_CompletionTimestamp] IS NOT NULL)
	RAISERROR('Revo infrastructure CREATE must be run first.', 20, 1)  WITH LOG;
GO

DECLARE @logId UNIQUEIDENTIFIER = NEWID();
DECLARE @timestamp DATETIME = GETDATE();
INSERT INTO [dbo].[REV_SCRIPT_LOG] ([REV_SCL_ScriptLogId],[REV_SCL_ScriptId], [REV_SCL_ScriptVersion],[REV_SCL_ScriptType],[REV_SCL_ScriptName],[REV_SCL_DbVersionId],[REV_SCL_StartTimestamp],[REV_SCL_CompletionTimestamp])
	VALUES (@logId, 'CDED157F-2538-4F33-81E0-4B2DD1BCC4AA', '1.0.0', 'STRUCTURE', '05_INS_PROGRAMMABILITY', 'DCB948CF-C9DB-41C9-AE9C-C08E97BC87B2', @timestamp, null);
GO

/***************************************************************/
IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[string_split]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
	DROP FUNCTION [dbo].[string_split]
GO

CREATE FUNCTION [dbo].[string_split] (@arg NVARCHAR(MAX),@delimiters NVARCHAR(100),@remove_empty_entries BIT)
RETURNS @outputTable TABLE (id INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,word NVARCHAR(MAX))
AS
BEGIN
	DECLARE @delimiters_table TABLE(delimiter NCHAR(1) UNIQUE CLUSTERED)
	DECLARE @i INT
	SET @i = 1
	DECLARE @len INT
	DECLARE @cur NCHAR(1)
	
	SET @len = LEN(@delimiters)
	IF LEN(@delimiters + 'x') <> @len + 1
		INSERT INTO @delimiters_table (delimiter) VALUES (N' ') /*it's hack to avoid the SQL's auto-trim (ignoring trailing spaces)*/
	WHILE @i <= @len	
	BEGIN
		SET @cur = SUBSTRING(@delimiters,@i,1);
		IF NOT EXISTS (SELECT 1 FROM @delimiters_table WHERE delimiter = @cur)
			BEGIN
				INSERT INTO @delimiters_table (delimiter) VALUES (@cur)
			END
		SET @i = @i + 1
	END
			
	DECLARE @placeholder NVARCHAR(4000)
	
	SET @i = 1
	SET @placeholder = N''
	SET @len = LEN(@arg + N'x') - 1
	WHILE @i <= @len
	BEGIN
	SET @cur = SUBSTRING(@arg,@i,1)
		IF NOT EXISTS (SELECT 1 FROM @delimiters_table WHERE delimiter=@cur)
		BEGIN
			SET @placeholder = @placeholder + @cur
			SET @i = @i+1
		END
		ELSE
			BEGIN
				IF (@placeholder <> N'') OR (@remove_empty_entries = 0)
				BEGIN
					INSERT INTO @outputTable(word) VALUES (LTRIM(RTRIM(@placeholder)))					
				END
				SET @placeholder = N''
				SET @i = @i + 1
			END	
	END	
	IF @placeholder <> N''
				BEGIN
					INSERT INTO @outputTable(word) VALUES (LTRIM(RTRIM(@placeholder)))					
				END
	
	RETURN	
END
GO


IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'COMPARE_VERSIONS')
	DROP FUNCTION [dbo].[COMPARE_VERSIONS]
GO

CREATE FUNCTION [dbo].[COMPARE_VERSIONS](@first VARCHAR(255), @second VARCHAR(255))
RETURNS INT
AS
BEGIN
	DECLARE @result INT
	;WITH	CTE(w1, i1, w2, i2) AS 
	(SELECT	FS.word, FS.id, SS.word, SS.id
	FROM	dbo.string_split(@first, '.',1) AS FS
			FULL OUTER JOIN
			dbo.string_split(@second, '.',1) AS SS
			ON FS.id = SS.id)
	SELECT	@result = CASE
				WHEN EXISTS (
								SELECT 1 FROM CTE WHERE w1 > w2
							)
					THEN 1
				WHEN EXISTS (
								SELECT 1 FROM CTE WHERE w1 > 0 AND w2 IS NULL
							)
					THEN 1
				WHEN EXISTS (
								SELECT 1 FROM CTE WHERE w1 < w2
							)
					THEN -1
				WHEN EXISTS (
								SELECT 1 FROM CTE WHERE w1 IS NULL AND 0 < w2
							)
					THEN -1
				ELSE 0
			END	
	RETURN @result
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'REV_SP_CheckScriptsRun')
DROP PROCEDURE [dbo].[REV_SP_CheckScriptsRun];
GO
CREATE PROCEDURE [dbo].[REV_SP_CheckScriptsRun] (@scriptsIds NVARCHAR(MAX))
AS
BEGIN
	DECLARE @scripts TABLE (SCRIPT_ID UNIQUEIDENTIFIER, SCRIPT_VERSION VARCHAR(255), SCRIPT_COMPARER VARCHAR(10))
	INSERT @scripts
	SELECT	(SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 1) AS SCRIPT_ID,
			(SELECT REPLACE(REPLACE(REPLACE(word, '<',''),'=',''),'>','') FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) AS SCRIPT_VERSION,
			CASE
				WHEN (SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) LIKE '%>%' THEN 'GT'
				WHEN (SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) LIKE '%>=%' THEN 'GEQ'
				WHEN (SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) LIKE '%<%' THEN 'LT'
				WHEN (SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) LIKE '%<=%' THEN 'LEQ'
				WHEN (SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) LIKE '%<>%' THEN 'NEQ'
				ELSE NULL
			END AS VERSION_COMPARER
	FROM	string_split(@scriptsIds, ';') AS SCRIPTS
	
	DECLARE @message VARCHAR(1024) = 'This script cannot be run because not all dependencies are satisfied.';

	IF EXISTS (
	SELECT	*
	FROM	(
				SELECT	*,
						(SELECT TOP 1 1
						FROM	[dbo].[REV_SCRIPT_LOG] AS LOGGED
						WHERE	REV_SCL_ScriptId = SCRIPTS.SCRIPT_ID
								AND
								(	1 =
									CASE
										WHEN SCRIPT_VERSION IS NULL THEN 1
										WHEN SCRIPT_COMPARER IS NULL THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) = 0 THEN 1 ELSE 0 END
										WHEN SCRIPT_COMPARER = 'GT' THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) = 1 THEN 1 ELSE 0 END
										WHEN SCRIPT_COMPARER = 'GEQ' THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) IN (1,0) THEN 1 ELSE 0 END							
										WHEN SCRIPT_COMPARER = 'LT' THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) = -1 THEN 1 ELSE 0 END							
										WHEN SCRIPT_COMPARER = 'LEQ' THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) IN (-1,0) THEN 1 ELSE 0 END							
										WHEN SCRIPT_COMPARER = 'NEQ' THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) IN (-1,1) THEN 1 ELSE 0 END	
									END						
								)
						) SATISFACTION
				FROM	@scripts AS SCRIPTS
			) AS SATISFACTION
			WHERE SATISFACTION.SATISFACTION IS NULL
			)
		RAISERROR(@message, 20, 1)  WITH LOG
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'REV_SP_CheckScriptsNotRun')
DROP PROCEDURE [dbo].[REV_SP_CheckScriptsNotRun];
GO
CREATE PROCEDURE [dbo].[REV_SP_CheckScriptsNotRun] (@scriptsIds NVARCHAR(MAX))
AS
BEGIN
	DECLARE @scripts TABLE (SCRIPT_ID UNIQUEIDENTIFIER, SCRIPT_VERSION VARCHAR(255), SCRIPT_COMPARER VARCHAR(10))
	INSERT @scripts
	SELECT	(SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 1) AS SCRIPT_ID,
			(SELECT REPLACE(REPLACE(REPLACE(word, '<',''),'=',''),'>','') FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) AS SCRIPT_VERSION,
			CASE
				WHEN (SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) LIKE '%>%' THEN 'GT'
				WHEN (SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) LIKE '%>=%' THEN 'GEQ'
				WHEN (SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) LIKE '%<%' THEN 'LT'
				WHEN (SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) LIKE '%<=%' THEN 'LEQ'
				WHEN (SELECT word FROM dbo.string_split(SCRIPTS.value,'@',1) WHERE id = 2) LIKE '%<>%' THEN 'NEQ'
				ELSE NULL
			END AS VERSION_COMPARER
	FROM	string_split(@scriptsIds, ';') AS SCRIPTS
	
	DECLARE @message VARCHAR(1024) = 'Some of scripts has been already run.';

	IF EXISTS (
	SELECT	*
	FROM	(
				SELECT	*,
						(SELECT TOP 1 1
						FROM	[dbo].[REV_SCRIPT_LOG] AS LOGGED
						WHERE	REV_SCL_ScriptId = SCRIPTS.SCRIPT_ID
								AND
								(	1 =
									CASE
										WHEN SCRIPT_VERSION IS NULL THEN 1
										WHEN SCRIPT_COMPARER IS NULL THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) = 0 THEN 1 ELSE 0 END
										WHEN SCRIPT_COMPARER = 'GT' THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) = 1 THEN 1 ELSE 0 END
										WHEN SCRIPT_COMPARER = 'GEQ' THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) IN (1,0) THEN 1 ELSE 0 END							
										WHEN SCRIPT_COMPARER = 'LT' THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) = -1 THEN 1 ELSE 0 END							
										WHEN SCRIPT_COMPARER = 'LEQ' THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) IN (-1,0) THEN 1 ELSE 0 END							
										WHEN SCRIPT_COMPARER = 'NEQ' THEN
											CASE WHEN dbo.COMPARE_VERSIONS(LOGGED.REV_SCL_ScriptVersion, SCRIPT_VERSION) IN (-1,1) THEN 1 ELSE 0 END	
									END						
								)
						) SATISFACTION
				FROM	@scripts AS SCRIPTS
			) AS SATISFACTION
			WHERE SATISFACTION.SATISFACTION IS NOT NULL
			)
		RAISERROR(@message, 20, 1)  WITH LOG
END
GO


IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'REV_SP_RunStructureScript')
DROP PROCEDURE [dbo].[REV_SP_RunStructureScript];
GO
CREATE PROCEDURE [dbo].[REV_SP_RunStructureScript] (@scriptId UNIQUEIDENTIFIER, @scriptName NVARCHAR(1024), @dbVersionId UNIQUEIDENTIFIER, @scriptVersion VARCHAR(255) = NULL)
AS
BEGIN
	IF EXISTS (
		SELECT 1 FROM [REV_SCRIPT_LOG] WHERE [REV_SCL_ScriptId] = @scriptId AND REV_SCL_CompletionTimestamp IS NULL
	)
		RAISERROR('Cannot run this script because it has been run yet and still is not completed.', 20, 1)  WITH LOG;

	DECLARE @logId UNIQUEIDENTIFIER = NEWID();
	DECLARE @timestamp DATETIME = GETDATE();
	INSERT INTO [dbo].[REV_SCRIPT_LOG] ([REV_SCL_ScriptLogId],[REV_SCL_ScriptId], [REV_SCL_ScriptVersion], [REV_SCL_ScriptType],[REV_SCL_ScriptName],[REV_SCL_DbVersionId],[REV_SCL_StartTimestamp],[REV_SCL_CompletionTimestamp])
	VALUES (@logId, @scriptId, @scriptVersion, 'STRUCTURE', @scriptName, @dbVersionId, @timestamp, null);
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'REV_SP_CompleteStructureScript')
	DROP PROCEDURE [dbo].[REV_SP_CompleteStructureScript];
GO

CREATE PROCEDURE [dbo].[REV_SP_CompleteStructureScript] (@scriptId UNIQUEIDENTIFIER, @scriptVersion VARCHAR(255) = NULL)
AS
BEGIN	
	DECLARE @timestamp DATETIME = GETDATE();
	UPDATE [dbo].[REV_SCRIPT_LOG]
	SET	[REV_SCL_CompletionTimestamp] = @timestamp
	WHERE	[REV_SCL_ScriptLogId] = (
		SELECT TOP 1 [REV_SCL_ScriptLogId]
		FROM	[REV_SCRIPT_LOG]
		WHERE	[REV_SCL_ScriptId] = @scriptId AND COALESCE([REV_SCL_ScriptVersion],'NULL') = COALESCE(@scriptVersion, 'NULL')
				AND [REV_SCL_CompletionTimestamp] IS NULL
	)
END
GO
/***********************************************************************************/
EXEC [dbo].[REV_SP_CompleteStructureScript] 'CDED157F-2538-4F33-81E0-4B2DD1BCC4AA', '1.0.0';