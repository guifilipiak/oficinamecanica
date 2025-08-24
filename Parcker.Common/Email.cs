using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

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
            _porta = porta;
        }

        public void EnviarEmail(string[] destinatarios, string assunto, string mensagem, Dictionary<string, Stream> arquivo = null)
        {
            using (var client = new SmtpClient())
            {
                client.Host = _servidor;
                client.EnableSsl = true;
                client.Port = _porta ?? 587;
                client.Credentials = new NetworkCredential(_usuario, _senha);

                using (var message = new MailMessage())
                {
                    message.IsBodyHtml = true;
                    message.Priority = MailPriority.High;
                    message.Body = mensagem;
                    message.From = new MailAddress(_remetente);

                    foreach (var destinatario in destinatarios)
                    {
                        message.To.Add(destinatario);
                    }

                    message.Subject = assunto;

                    if (arquivo != null && arquivo.Count > 0)
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
    }
}