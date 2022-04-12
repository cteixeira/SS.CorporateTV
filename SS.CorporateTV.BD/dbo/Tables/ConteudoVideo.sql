CREATE TABLE [dbo].[ConteudoVideo] (
    [ConteudoVideoID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [ConteudoID]      BIGINT         NOT NULL,
    [Designacao]      NVARCHAR (200) NOT NULL,
    [Url]             NVARCHAR (200) NOT NULL,
    [Duracao]         INT            NULL,
    [Ordem]           INT            NULL,
    CONSTRAINT [PK_ConteudoVideo] PRIMARY KEY CLUSTERED ([ConteudoVideoID] ASC),
    CONSTRAINT [FK_ConteudoVideo_Conteudo] FOREIGN KEY ([ConteudoID]) REFERENCES [dbo].[Conteudo] ([ConteudoID]) ON DELETE CASCADE
);





