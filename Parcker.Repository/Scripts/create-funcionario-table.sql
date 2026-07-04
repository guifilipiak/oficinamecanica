-- Script para criar tabela Funcionario e adicionar campo na OrdemServico
CREATE TABLE Funcionario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    CPF VARCHAR(14) NOT NULL,
    Telefone VARCHAR(20) NULL,
    Email VARCHAR(100) NULL,
    Cargo VARCHAR(50) NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    DataCriacao DATETIME NOT NULL DEFAULT GETDATE()
);

-- Adicionar campo IdFuncionario na tabela OrdemServico
ALTER TABLE OrdemServico 
ADD IdFuncionario INT NULL;

-- Criar foreign key (opcional)
-- ALTER TABLE OrdemServico 
-- ADD CONSTRAINT FK_OrdemServico_Funcionario 
-- FOREIGN KEY (IdFuncionario) REFERENCES Funcionario(Id);