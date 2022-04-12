CREATE TABLE [dbo].[Utilizador] (
    [UtilizadorID]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [EmpresaID]        BIGINT         NOT NULL,
    [Nome]             NVARCHAR (200) NOT NULL,
    [Username]         NVARCHAR (200) NOT NULL,
    [Password]         NVARCHAR (200) NULL,
    [PerfilUtilizador] SMALLINT       NOT NULL,
    CONSTRAINT [PK_Utilizador] PRIMARY KEY CLUSTERED ([UtilizadorID] ASC),
    CONSTRAINT [FK_Utilizador_Utilizador] FOREIGN KEY ([EmpresaID]) REFERENCES [dbo].[Empresa] ([EmpresaID])
);




GO


