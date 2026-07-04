-- Criação das tabelas para o sistema de checklist

-- Tabela ChecklistVeiculo
CREATE TABLE ChecklistVeiculo (
    Id int IDENTITY(1,1) PRIMARY KEY,
    IdOrdemServico int NOT NULL,
    Observacoes nvarchar(1000) NULL,
    DataCriacao datetime NOT NULL,
    FOREIGN KEY (IdOrdemServico) REFERENCES OrdemServico(Id)
);

-- Tabela ChecklistItem
CREATE TABLE ChecklistItem (
    Id int IDENTITY(1,1) PRIMARY KEY,
    IdChecklistVeiculo int NOT NULL,
    Sistema nvarchar(100) NOT NULL,
    Item nvarchar(200) NOT NULL,
    Verificado bit NOT NULL DEFAULT 0,
    Observacao nvarchar(500) NULL,
    DataCriacao datetime NOT NULL,
    FOREIGN KEY (IdChecklistVeiculo) REFERENCES ChecklistVeiculo(Id) ON DELETE CASCADE
);

-- Índices para melhor performance
CREATE INDEX IX_ChecklistVeiculo_IdOrdemServico ON ChecklistVeiculo(IdOrdemServico);
CREATE INDEX IX_ChecklistItem_IdChecklistVeiculo ON ChecklistItem(IdChecklistVeiculo);
CREATE INDEX IX_ChecklistItem_Sistema ON ChecklistItem(Sistema);