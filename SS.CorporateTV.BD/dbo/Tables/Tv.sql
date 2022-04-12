CREATE TABLE [dbo].[Tv] (
    [TvID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [EmpresaID]     BIGINT         NOT NULL,
    [Designacao]    NVARCHAR (200) NOT NULL,
    [ProgramacaoID] BIGINT         NOT NULL,
    CONSTRAINT [PK_Tv] PRIMARY KEY CLUSTERED ([TvID] ASC),
    CONSTRAINT [FK_Tv_Empresa] FOREIGN KEY ([EmpresaID]) REFERENCES [dbo].[Empresa] ([EmpresaID]),
    CONSTRAINT [FK_Tv_Programacao] FOREIGN KEY ([ProgramacaoID]) REFERENCES [dbo].[Programacao] ([ProgramacaoID])
);






GO



GO


