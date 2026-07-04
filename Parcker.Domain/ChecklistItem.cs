namespace Parcker.Domain
{
    public class ChecklistItem : Base
    {
        public virtual int IdChecklistVeiculo { get; set; }
        public virtual string Sistema { get; set; }
        public virtual string Item { get; set; }
        public virtual bool Verificado { get; set; }
        public virtual string Observacao { get; set; }
    }
}