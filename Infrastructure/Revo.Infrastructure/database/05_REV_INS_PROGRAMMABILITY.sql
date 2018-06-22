/*
ModuleId: 5F08DF74-3C4B-4DBD-AD35-26565239E157
Version: 1.2.0
ScriptId: CDED157F-2538-4F33-81E0-4B2DD1BCC4AA
*/
EXEC [dbo].[REV_SP_CheckScriptsRun] '1FBA6358-7AC1-4B6F-82D6-48BBCBD5C99E';
GO
EXEC [dbo].[REV_SP_RunStructureScript] 'CDED157F-2538-4F33-81E0-4B2DD1BCC4AA', '03_REV_INS_PROGRAMMABILITY', '2D3C7567-2905-4466-972C-C38204FA7F18', '1.2.0';
GO

/***************************************************************/

/***************************************************************/
EXEC [dbo].[REV_SP_CompleteStructureScript] 'CDED157F-2538-4F33-81E0-4B2DD1BCC4AA', '1.2.0';