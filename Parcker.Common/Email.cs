using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Common
{
    public class Email
    {
        private string _servidor;
        private string _remetente;
        private string _usuario;
        private string _senha;
        private int? _porta;
        public Email(string servidor, int? porta, string remetente, string usuario, string senha)
        {
            _remetente = remetente;
            _usuario = usuario;
            _senha = senha;
            _servidor = servidor;
        }

        public void EnviarEmail(string[] destinatarios, string assunto, string mensagem, Dictionary<string, Stream> arquivo)
        {
            SmtpClient client = new SmtpClient();
            client.Host = _servidor;
            client.EnableSsl = true;
            client.Port = _porta.HasValue ? _porta.Value : 587;
            client.Credentials = new NetworkCredential()
            {
                UserName = _usuario,
                Password = _senha
            };

            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;
            message.Body = mensagem;
            message.From = new MailAddress(_remetente);
            message.To.Add(string.Join(",", destinatarios));
            message.Subject = assunto;

            if (arquivo.Count > 0)
            {
                foreach (var file in arquivo)
                {
                    message.Attachments.Add(new Attachment(file.Value, file.Key));
                }
            }

            client.Send(message);
        }
    }
}
