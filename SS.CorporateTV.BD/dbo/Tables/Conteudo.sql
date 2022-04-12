CREATE TABLE [dbo].[Conteudo] (
    [ConteudoID]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [EmpresaID]     BIGINT         NULL,
    [Designacao]    NVARCHAR (200) NOT NULL,
    [Tipo]          SMALLINT       NOT NULL,
    [QRCode]        NVARCHAR (200) NULL,
    [DataAlteracao] DATETIME       NULL,
    CONSTRAINT [PK_Conteudo] PRIMARY KEY CLUSTERED ([ConteudoID] ASC),
    CONSTRAINT [FK_Conteudo_Empresa] FOREIGN KEY ([EmpresaID]) REFERENCES [dbo].[Empresa] ([EmpresaID])
);






GO


