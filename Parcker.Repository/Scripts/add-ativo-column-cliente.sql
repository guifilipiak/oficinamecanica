-- Script para adicionar coluna Ativo na tabela Cliente

-- Adicionar coluna Ativo
ALTER TABLE dbo.Cliente 
ADD Ativo BIT NOT NULL DEFAULT 1;

-- Comentário da coluna
EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = N'Status ativo do cliente',
    @level0type = N'Schema', @level0name = 'dbo',
    @level1type = N'Table', @level1name = 'Cliente',
    @level2type = N'Column', @level2name = 'Ativo';

-- Criar índice para performance
CREATE NONCLUSTERED INDEX IX_Cliente_Ativo ON dbo.Cliente (Ativo);