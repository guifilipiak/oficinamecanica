using System.Web;
using System.Web.Optimization;

namespace Parcker
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/layout-flat/css").Include(
                      "~/template/vendor/fontawesome-free/css/all.min.css",
                      "~/Content/fonts-google.css",
                      "~/template/css/sb-admin-2.min.css"));

            bundles.Add(new ScriptBundle("~/layout-flat/js").Include(
                      "~/template/vendor/jquery/jquery.min.js",
                      "~/template/vendor/bootstrap/js/bootstrap.bundle.min.js",
                      "~/template/vendor/jquery-easing/jquery.easing.min.js",
                      "~/template/js/sb-admin-2.min.js"));

            bundles.Add(new StyleBundle("~/layout/css").Include(
                      "~/template/vendor/fontawesome-free/css/all.min.css",
                      "~/Content/fonts-google.css",
                      "~/template/css/sb-admin-2.min.css",
                      "~/template/vendor/toast-master/css/jquery.toast.css",
                      "~/template/vendor/sweetalert2/sweetalert2.css",
                      "~/template/vendor/calendario/css/bootstrap-datepicker3.css",
                      "~/Content/animate.css",
                      "~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/layout/js").Include(
                      "~/template/vendor/jquery/jquery.min.js",
                      "~/template/vendor/bootstrap/js/bootstrap.bundle.min.js",
                      "~/template/vendor/toast-master/js/jquery.toast.js",
                      "~/template/vendor/sweetalert2/sweetalert2.js",
                      "~/template/vendor/calendario/js/bootstrap-datepicker.js",
                      "~/template/vendor/calendario/locales/bootstrap-datepicker.pt-BR.min.js",
                      "~/template/vendor/jquery-easing/jquery.easing.min.js",
                      "~/template/js/sb-admin-2.min.js",
                      "~/Scripts/jquery.mask.js",
                      "~/Scripts/geral.js"));

            //plugins
            bundles.Add(new ScriptBundle("~/jquery/validate").Include(
                      "~/Scripts/jquery.validate.min.js",
                      "~/Scripts/jquery.validate.pt-br.min.js",
                      "~/Scripts/jquery.validate.unobtrusive.min.js",
                      "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new StyleBundle("~/plugin/dtable/css").Include(
                      "~/template/vendor/datatables/dataTables.bootstrap4.min.css"));

            bundles.Add(new ScriptBundle("~/plugin/dtable/js").Include(
                      "~/template/vendor/datatables/jquery.dataTables.min.js",
                      "~/template/vendor/datatables/dataTables.bootstrap4.min.js"));

            bundles.Add(new ScriptBundle("~/plugin/graph/js").Include(
                      "~/template/vendor/chart.js/Chart.min.js",
                      "~/template/js/demo/chart-area-demo.js",
                      "~/template/js/demo/chart-pie-demo.js"));

            bundles.Add(new StyleBundle("~/plugin/autocomplete/css").Include(
                      "~/template/vendor/autocomplete/jquery.auto-complete.css",
                      "~/template/vendor/ajax-autocomplete/bootstrap-select.min.css",
                      "~/template/vendor/ajax-autocomplete/ajax-bootstrap-select.min.css"));

            bundles.Add(new ScriptBundle("~/plugin/autocomplete/js").Include(
                      "~/template/vendor/autocomplete/jquery.auto-complete.min.js",
                      "~/template/vendor/ajax-autocomplete/bootstrap-select.min.js",
                      "~/template/vendor/ajax-autocomplete/ajax-bootstrap-select.min.js",
                      "~/template/vendor/ajax-autocomplete/ajax-bootstrap-select.pt-BR.min.js"));

            //pages
            bundles.Add(new ScriptBundle("~/views/alerta/js").Include(
                      "~/Scripts/views/alerta/alerta.js"));

            bundles.Add(new ScriptBundle("~/views/categoria/js").Include(
                      "~/Scripts/views/categoria/categoria.js"));

            bundles.Add(new ScriptBundle("~/views/cliente/js").Include(
                      "~/Scripts/views/cliente/cliente.js"));

            bundles.Add(new ScriptBundle("~/views/cupomdesconto/js").Include(
                      "~/Scripts/views/cupomdesconto/cupomdesconto.js"));

            bundles.Add(new ScriptBundle("~/views/fornecedor/js").Include(
                      "~/Scripts/views/fornecedor/fornecedor.js"));

            bundles.Add(new ScriptBundle("~/views/home/js").Include(
                      "~/Scripts/views/home/home.js"));

            bundles.Add(new ScriptBundle("~/views/marca/js").Include(
                      "~/Scripts/views/marca/marca.js"));

            bundles.Add(new ScriptBundle("~/views/ordemservico/js").Include(
                      "~/Scripts/views/ordemservico/ordemservico.js"));

            bundles.Add(new ScriptBundle("~/views/produto/js").Include(
                      "~/Scripts/views/produto/produto.js"));

            bundles.Add(new ScriptBundle("~/views/servico/js").Include(
                      "~/Scripts/views/servico/servico.js"));

            bundles.Add(new ScriptBundle("~/views/usuario/js").Include(
                      "~/Scripts/views/usuario/usuario.js"));

            bundles.Add(new ScriptBundle("~/views/veiculo/js").Include(
                      "~/Scripts/views/veiculo/veiculo.js"));

            bundles.Add(new ScriptBundle("~/views/pagarreceber/js").Include(
                      "~/Scripts/views/pagarreceber/pagarreceber.js"));

            bundles.Add(new ScriptBundle("~/views/pagarreceber/relatoriofluxocaixa/js").Include(
                      "~/Scripts/views/pagarreceber/relatoriofluxocaixa.js"));

            bundles.Add(new ScriptBundle("~/impressao/js").Include(
                      "~/template/vendor/jquery/jquery.min.js",
                      "~/template/vendor/toast-master/js/jquery.toast.js",
                      "~/template/vendor/sweetalert2/sweetalert2.js",
                      "~/Scripts/views/ordemservico/email.js"));

            bundles.Add(new StyleBundle("~/impressao/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/paper.css",
                      "~/template/vendor/toast-master/css/jquery.toast.css",
                      "~/template/vendor/sweetalert2/sweetalert2.css",
                      "~/template/vendor/fontawesome-free/css/all.min.css"));
        }
    }
}
