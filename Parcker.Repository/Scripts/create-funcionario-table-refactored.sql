-- Script para criação da tabela Funcionario refatorada
-- Usando referência à tabela Pessoa

-- Remover tabela existente se houver (cuidado em produção)
IF OBJECT_ID('dbo.Funcionario', 'U') IS NOT NULL
    DROP TABLE dbo.Funcionario;

-- Criar tabela Funcionario
CREATE TABLE dbo.Funcionario (
    Id INT IDENTITY(1,1) NOT NULL,
    IdPessoa INT NOT NULL,
    Cargo NVARCHAR(100) NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    DataCriacao DATETIME NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_Funcionario PRIMARY KEY (Id),
    CONSTRAINT FK_Funcionario_Pessoa FOREIGN KEY (IdPessoa) REFERENCES dbo.Pessoa(Id)
);

-- Criar índices
CREATE NONCLUSTERED INDEX IX_Funcionario_IdPessoa ON dbo.Funcionario (IdPessoa);
CREATE NONCLUSTERED INDEX IX_Funcionario_Ativo ON dbo.Funcionario (Ativo);
CREATE NONCLUSTERED INDEX IX_Funcionario_Cargo ON dbo.Funcionario (Cargo);

-- Comentários das colunas
EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = N'Identificador único do funcionário',
    @level0type = N'Schema', @level0name = 'dbo',
    @level1type = N'Table', @level1name = 'Funcionario',
    @level2type = N'Column', @level2name = 'Id';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = N'Referência para a tabela Pessoa',
    @level0type = N'Schema', @level0name = 'dbo',
    @level1type = N'Table', @level1name = 'Funcionario',
    @level2type = N'Column', @level2name = 'IdPessoa';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = N'Cargo do funcionário',
    @level0type = N'Schema', @level0name = 'dbo',
    @level1type = N'Table', @level1name = 'Funcionario',
    @level2type = N'Column', @level2name = 'Cargo';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = N'Status ativo do funcionário',
    @level0type = N'Schema', @level0name = 'dbo',
    @level1type = N'Table', @level1name = 'Funcionario',
    @level2type = N'Column', @level2name = 'Ativo';