-- Criar tabela TipoAtendimento
CREATE TABLE TipoAtendimento (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Descricao NVARCHAR(100) NOT NULL,
    DataCriacao DATETIME NOT NULL DEFAULT GETDATE()
);

-- Inserir dados iniciais
INSERT INTO TipoAtendimento (Descricao, DataCriacao) VALUES 
('Ordem de Serviço', GETDATE()),
('Orçamento', GETDATE());

-- Adicionar coluna IdTipoAtendimento na tabela OrdemServico
ALTER TABLE OrdemServico 
ADD IdTipoAtendimento INT NOT NULL DEFAULT 1;

-- Alterar colunas para permitir NULL quando orçamento
ALTER TABLE OrdemServico 
ALTER COLUMN IdVeiculo INT NULL;

ALTER TABLE OrdemServico 
ALTER COLUMN IdCliente INT NULL;

-- Criar foreign key
ALTER TABLE OrdemServico 
ADD CONSTRAINT FK_OrdemServico_TipoAtendimento 
FOREIGN KEY (IdTipoAtendimento) REFERENCES TipoAtendimento(Id);