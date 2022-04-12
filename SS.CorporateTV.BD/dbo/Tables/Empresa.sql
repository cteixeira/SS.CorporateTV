CREATE TABLE [dbo].[Empresa] (
    [EmpresaID]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [Designacao]             NVARCHAR (200) NOT NULL,
    [Url]                    NVARCHAR (200) NOT NULL,
    [Logo]                   NVARCHAR (MAX) NULL,
    [Cor1]                   NVARCHAR (8)   NULL,
    [Cor2]                   NVARCHAR (8)   NULL,
    [Cor3]                   NVARCHAR (8)   NULL,
    [TempoRefrescarConteudo] SMALLINT       NOT NULL,
    [Activo]                 BIT            NOT NULL,
    CONSTRAINT [PK_Empresa] PRIMARY KEY CLUSTERED ([EmpresaID] ASC)
);



