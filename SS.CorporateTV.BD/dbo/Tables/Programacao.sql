CREATE TABLE [dbo].[Programacao] (
    [ProgramacaoID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [EmpresaID]     BIGINT        NOT NULL,
    [Designacao]    NVARCHAR (50) NOT NULL,
    [DataAlteracao] DATETIME      NULL,
    CONSTRAINT [PK_Programacao] PRIMARY KEY CLUSTERED ([ProgramacaoID] ASC),
    CONSTRAINT [FK_Programacao_Empresa] FOREIGN KEY ([EmpresaID]) REFERENCES [dbo].[Empresa] ([EmpresaID])
);






GO


