CREATE TABLE [dbo].[ConteudoImagem] (
    [ConteudoImagemID] BIGINT          IDENTITY (1, 1) NOT NULL,
    [ConteudoID]       BIGINT          NOT NULL,
    [UrlImagem]        NVARCHAR (200)  NULL,
    [Duracao]          INT             NOT NULL,
    [Extensao]         NVARCHAR (10)   NOT NULL,
    [Imagem]           VARBINARY (MAX) NOT NULL,
    [HashFicheiro]     VARBINARY (130) NULL,
    [Designacao]       NVARCHAR (200)  NOT NULL,
    [Ordem]            INT             NULL,
    CONSTRAINT [PK_ConteudoImagem] PRIMARY KEY CLUSTERED ([ConteudoImagemID] ASC),
    CONSTRAINT [FK_ConteudoImagem_Conteudo] FOREIGN KEY ([ConteudoID]) REFERENCES [dbo].[Conteudo] ([ConteudoID]) ON DELETE CASCADE
);





