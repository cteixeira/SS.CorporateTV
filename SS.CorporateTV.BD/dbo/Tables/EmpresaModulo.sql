CREATE TABLE [dbo].[EmpresaModulo] (
    [EmpresaModuloID] BIGINT   IDENTITY (1, 1) NOT NULL,
    [EmpresaID]       BIGINT   NOT NULL,
    [Modulo]          SMALLINT NOT NULL,
    CONSTRAINT [PK_EmpresaModulo] PRIMARY KEY CLUSTERED ([EmpresaModuloID] ASC),
    CONSTRAINT [FK_EmpresaModulo_Empresa] FOREIGN KEY ([EmpresaID]) REFERENCES [dbo].[Empresa] ([EmpresaID])
);




GO


