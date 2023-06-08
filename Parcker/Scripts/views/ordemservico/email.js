var id_email_os;
function enviarEmailOS(id, destinatario) {
    id_email_os = id;
    Swal.fire({
        title: 'Deseja enviar esta OS por e-mail?',
        text: 'Todas as informações da OS serão enviadas para o cliente informado na OS.',
        type: 'info',
        showCancelButton: true,
        confirmButtonText: 'Sim, desejo enviar!',
        cancelButtonText: 'Não, quero cancelar!'
    }).then((result) => {
        if (result.value) {
            Swal.fire({
                title: 'Enviando E-Mail',
                html: 'Aguarde...',
                type: 'info',
                allowOutsideClick: false,
                onBeforeOpen: () => {
                    Swal.showLoading();
                }
            });
            $.ajax({
                url: URLBase + "OrdemServico/EnviarEmailOS",
                type: "POST",
                data: {
                    id: id_email_os,
                    destinatario: destinatario
                },
                success: function (result) {
                    if (result.IsValid && result.IsHaveEmail) {
                        Swal.fire(
                            'OK',
                            result.Message,
                            'success'
                        );
                        Swal.hideLoading();
                    } else {
                        Swal.fire(
                            'Atenção',
                            result.Message,
                            'warning'
                        );
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    Swal.fire(
                        'Vish',
                        'Algo de errado aconteceu, tente novamente ou entre em contato com o administrador do site!',
                        'error'
                    );
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            //cancelado
        }
    });
}