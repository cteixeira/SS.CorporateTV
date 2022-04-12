CREATE TABLE [dbo].[ProgramacaoAgendamento] (
    [ProgramacaoAgendamentoID] BIGINT   IDENTITY (1, 1) NOT NULL,
    [ProgramacaoID]            BIGINT   NOT NULL,
    [ConteudoID]               BIGINT   NOT NULL,
    [DiaSemana]                SMALLINT NOT NULL,
    [Inicio]                   TIME (7) NOT NULL,
    [Fim]                      TIME (7) NOT NULL,
    CONSTRAINT [PK_ProgramacaoAgendamento] PRIMARY KEY CLUSTERED ([ProgramacaoAgendamentoID] ASC),
    CONSTRAINT [FK_ProgramacaoAgendamento_Conteudo] FOREIGN KEY ([ConteudoID]) REFERENCES [dbo].[Conteudo] ([ConteudoID]),
    CONSTRAINT [FK_ProgramacaoAgendamento_Programacao] FOREIGN KEY ([ProgramacaoID]) REFERENCES [dbo].[Programacao] ([ProgramacaoID])
);




GO



GO


