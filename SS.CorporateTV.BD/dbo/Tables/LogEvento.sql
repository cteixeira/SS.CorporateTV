CREATE TABLE [dbo].[LogEvento] (
    [LogEventoID]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [EventoID]      INT            NOT NULL,
    [UtilizadorID]  BIGINT         NOT NULL,
    [Tabela]        NVARCHAR (50)  NOT NULL,
    [Campo]         NVARCHAR (50)  NULL,
    [Identificador] NCHAR (10)     NOT NULL,
    [ValorNovo]     NVARCHAR (300) NULL,
    [ValorAnterior] NVARCHAR (300) NULL,
    [DataHora]      DATETIME       NOT NULL,
    CONSTRAINT [PK_LogEvento] PRIMARY KEY CLUSTERED ([LogEventoID] ASC)
);



