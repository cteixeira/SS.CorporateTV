CREATE TABLE [dbo].[LogErro] (
    [LogErroID]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Importancia] BIGINT         NULL,
    [Origem]      NVARCHAR (MAX) NULL,
    [Descricao]   NVARCHAR (MAX) NULL,
    [DataHora]    DATETIME       NULL,
    [UserName]    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_LogErro] PRIMARY KEY CLUSTERED ([LogErroID] ASC)
);



