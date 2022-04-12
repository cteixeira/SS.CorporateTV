CREATE TABLE [dbo].[Evento] (
    [id]         INT          IDENTITY (1, 1) NOT NULL,
    [text]       VARCHAR (50) NOT NULL,
    [eventstart] DATETIME     NOT NULL,
    [eventend]   DATETIME     NOT NULL,
    CONSTRAINT [PK_Evento] PRIMARY KEY CLUSTERED ([id] ASC)
);



