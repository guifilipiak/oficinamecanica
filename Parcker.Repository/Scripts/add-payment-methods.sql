-- Script para adicionar campos de forma de pagamento na tabela OrdemServico
ALTER TABLE OrdemServico 
ADD PagamentoDinheiro BIT NULL,
    PagamentoCartaoDebito BIT NULL,
    PagamentoCartaoCredito BIT NULL,
    PagamentoPix BIT NULL,
    ObservacoesPagamento VARCHAR(500) NULL;