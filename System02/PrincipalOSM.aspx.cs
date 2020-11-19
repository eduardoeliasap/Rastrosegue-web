using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class System02_Principal___OSM : System.Web.UI.Page
{
    Avisos objAvisos = new Avisos();
    Saldos objSaldo = new Saldos();
    Veiculos objVeiculos = new Veiculos();
    Localizacao objLocalizacao = new Localizacao();
    Pagamentos objPagamentos = new Pagamentos();
    Mensagens objMensagens = new Mensagens();
    Login objLogin = new Login();
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptManager1.RegisterPostBackControl(txtLatitude);
        ScriptManager1.RegisterPostBackControl(txtLongitude);
        ScriptManager1.RegisterPostBackControl(txtVeiculo);
        ScriptManager1.RegisterPostBackControl(ddlVeiculos);
        ScriptManager1.RegisterPostBackControl(gvLocalizacoes);
        ScriptManager1.RegisterPostBackControl(btnExibeRelData);
        ScriptManager1.RegisterPostBackControl(txtTextoPin); 

        try
        {
            if (!Page.IsPostBack)
            {
                System.Web.HttpBrowserCapabilities browser = Request.Browser;
                if (browser.Browser == "IE")
                    ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Este navegador não é compatível. Por favor utilize o Google Chrome para melhor desempenho do sistema") + "');</script>");                

                
                ddlVeiculosCere.Visible = false;
                ddlVeiculos.Visible = true;                

                btnLocalizar.Enabled = false;
                btnCarregaLocal.Enabled = false;

                btnParaLocalizacao.Visible = false;
                btnRelData.Enabled = false;

                // Retorna os dados do usuário
                DataTable dt = objLogin.DadosCliente(Convert.ToInt32(Session["mCodCliente"].ToString()));
                if (Convert.ToBoolean(dt.Rows[0]["Usu_HabilitaFrota"].ToString()))
                    btnFrota.Visible = true;
                else
                    btnFrota.Visible = false; 

                txtLatitude.Text = "0";
                txtLongitude.Text = "0";
                txtIconeColor.Text = "#00ff00";
                txtZoom.Text = "2";

                pnAviso01.Visible = false;
                pnRelData.Visible = true;

                lblLoginUsuario.Text = Session["mLogin"].ToString().TrimEnd();
                lblNomeCompleto.Text = Session["mNomeCompleto"].ToString().TrimEnd();

                CarregaFaturaAberta();
                CarregaAvisos();
                CarregaMensagens();

                chAtualizacaoPagina.Checked = false;

                TimerLocalizacoes.Enabled = true;
                btnRefresh.Visible = false;
                btnRefreshNo.Visible = true;                

                ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMap();</script>");                

                if (Session["mRelData"].ToString() == "true")
                { HabilitaRelData(); Session["mData"] = "false"; }
            }                        
        }
        catch (Exception err)
        { ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Erro: " + err) + "');</script>"); } 
    }
    protected string ConvertHoras(int mMinutos)
    {
        return (mMinutos / 60).ToString();
    }
    protected void CarregaFaturaAberta()
    {
        // Verifica as faturas em aberto
        DataTable dt = objPagamentos.ParcelasCliente(Convert.ToInt32(Session["mCodCliente"].ToString()));
        if (dt != null && dt.Rows.Count > 0)
        {
            if (dt.Rows[0]["Pag_Status"].ToString().Replace(" ", "") == "Pendente")
            {
                string[] mVetData = dt.Rows[0]["Pag_DtVencimento"].ToString().Split(' ');
                lblTotalFatura.Text = "R$ " + dt.Rows[0]["Pag_Valor"].ToString();
                lblVencimento.Text = mVetData[0];

                lblStatus.Text = "Disponível para pagamento";                
            }
            else
                pnFatura.Visible = false;
        }
        else
            pnFatura.Visible = false;
    }
    protected void CarregaAvisos()
    {
        try
        {
            pnAviso01.Visible = false;
            pnAviso02.Visible = false;
            pnAviso03.Visible = false;
            pnAviso04.Visible = false;

            lblTotalAvisos.Text = string.Empty;
            lblTotalAvisosPendentes.Text = string.Empty;

            // Retorna os quatro últimos avisos
            DataTable dt = objAvisos.RetornaAvisosVeiculo(Convert.ToInt32(ddlVeiculos.SelectedValue));
            if (dt != null && dt.Rows.Count > 0)
            {
                int mHoras = 0;
                hplTodosAvisos.NavigateUrl = "Doc-Avisos.aspx?mVeiculo=" + ddlVeiculos.SelectedValue;

                lblTotalAvisosPendentes.Text = dt.Rows.Count.ToString();
                lblTotalAvisos.Text = "Você tem " + dt.Rows.Count.ToString() + " avisos pendentes";
                if (dt.Rows.Count >= 1)
                {
                    mHoras = Convert.ToInt32(ConvertHoras(Convert.ToInt32(dt.Rows[0]["Minutos"].ToString())));
                    lblAviso01.Text = dt.Rows[0]["Avi_Mensagem"].ToString().TrimEnd();
                    lblAvisoMin01.Text = mHoras >= 1 ? mHoras + " horas" : dt.Rows[0]["Minutos"].ToString() + " min";

                    if (mHoras >= 24 && mHoras <= 48) { lblAvisoMin01.Text = " ontem"; }
                    if (mHoras > 48) { lblAvisoMin01.Text = mHoras / 24 + " dias atrás"; }

                    pnAviso01.Visible = true;
                }
                if (dt.Rows.Count >= 2)
                {
                    mHoras = Convert.ToInt32(ConvertHoras(Convert.ToInt32(dt.Rows[1]["Minutos"].ToString())));
                    lblAviso02.Text = dt.Rows[1]["Avi_Mensagem"].ToString().TrimEnd();
                    lblAvisoMin02.Text = mHoras >= 1 ? mHoras + " horas" : dt.Rows[1]["Minutos"].ToString() + " min";

                    if (mHoras >= 24 && mHoras <= 48) { lblAvisoMin02.Text = " ontem"; }
                    if (mHoras > 48) { lblAvisoMin02.Text = mHoras / 24 + " dias atrás"; }

                    pnAviso02.Visible = true;
                }
                if (dt.Rows.Count >= 3)
                {
                    mHoras = Convert.ToInt32(ConvertHoras(Convert.ToInt32(dt.Rows[2]["Minutos"].ToString())));
                    lblAviso03.Text = dt.Rows[2]["Avi_Mensagem"].ToString().TrimEnd();
                    lblAvisoMin03.Text = mHoras >= 1 ? mHoras + " horas" : dt.Rows[2]["Minutos"].ToString() + " min";

                    if (mHoras >= 24 && mHoras <= 48) { lblAvisoMin03.Text = " ontem"; }
                    if (mHoras > 48) { lblAvisoMin03.Text = mHoras / 24 + " dias atrás"; }

                    pnAviso03.Visible = true;
                }
                if (dt.Rows.Count >= 4)
                {
                    mHoras = Convert.ToInt32(ConvertHoras(Convert.ToInt32(dt.Rows[3]["Minutos"].ToString())));
                    lblAviso04.Text = dt.Rows[3]["Avi_Mensagem"].ToString().TrimEnd();
                    lblAvisoMin04.Text = mHoras >= 1 ? mHoras + " horas" : dt.Rows[3]["Minutos"].ToString() + " min";

                    if (mHoras >= 24 && mHoras <= 48) { lblAvisoMin04.Text = " ontem"; }
                    if (mHoras > 48) { lblAvisoMin04.Text = mHoras / 24 + " dias atrás"; }

                    pnAviso04.Visible = true;
                }
            }
        }
        catch (Exception err)
        { }
    }

    protected void CarregaMensagensUrgentes()
    {
        pnMensagens01.Visible = false;
        pnMensagens02.Visible = false;
        pnMensagens03.Visible = false;

        // Retorna mensagens para o painel do cliente
        DataTable dt = objMensagens.RetornaMensagensUsuarioUrgente(Convert.ToInt32(Session["mCodCliente"].ToString()));
        if (dt != null && dt.Rows.Count > 0)
        {
            int mHoras = 0;
            mHoras = Convert.ToInt32(ConvertHoras(Convert.ToInt32(dt.Rows[0]["Minutos"].ToString())));

            lblTituloMens01.Text = dt.Rows[0]["Men_Titulo"].ToString();
            lblHoraMens01.Text = mHoras >= 1 ? mHoras + " horas" : dt.Rows[0]["Minutos"].ToString() + " min";

            if (mHoras >= 24 && mHoras <= 48) { lblHoraMens01.Text = " ontem"; }
            if (mHoras > 48) { lblHoraMens01.Text = mHoras / 24 + " dias"; }

            lblPopTituloMens01.Text = dt.Rows[0]["Men_Titulo"].ToString();
            lblPopMensagem01.Text = dt.Rows[0]["Men_Descricao"].ToString();
            lblPopUpInteracao01.Text = "<b>Interação: </b>" + dt.Rows[0]["Men_Sequencia"].ToString();
            lblPopUpData01.Text = "<b>Data: </b>" + dt.Rows[0]["Men_Data"].ToString();

            pnMensagens01.Visible = true;
        }
    }
    protected void CarregaMensagens()
    {
        pnMensagens01.Visible = false;
        pnMensagens02.Visible = false;
        pnMensagens03.Visible = false;

        DataTable dt = objMensagens.RetornaMensagensUsuario(Convert.ToInt32(Session["mCodCliente"].ToString()));
        if (dt != null && dt.Rows.Count > 0)
        {
            lblTotalMens.Text = "Mensagens pendentes " + dt.Rows.Count.ToString();
            lblTotalNumMens.Text = dt.Rows.Count.ToString();

            int mHoras = 0;

            if (dt.Rows.Count >= 1)
            {
                mHoras = Convert.ToInt32(ConvertHoras(Convert.ToInt32(dt.Rows[0]["Minutos"].ToString())));

                lblTituloMens01.Text = dt.Rows[0]["Men_Titulo"].ToString() + ", ";
                lblHoraMens01.Text = mHoras >= 1 ? mHoras + " horas" : dt.Rows[0]["Minutos"].ToString() + " min atrás";

                if (mHoras >= 24 && mHoras <= 48) { lblHoraMens01.Text = " ontem"; }
                if (mHoras > 48) { lblHoraMens01.Text = mHoras / 24 + " dias atrás"; }

                lblPopTituloMens01.Text = dt.Rows[0]["Men_Titulo"].ToString();
                lblPopMensagem01.Text = dt.Rows[0]["Men_Descricao"].ToString();
                lblPopUpInteracao01.Text = "<b>Interação: </b>" + dt.Rows[0]["Men_Sequencia"].ToString();
                lblPopUpData01.Text = "<b>Data: </b>" + dt.Rows[0]["Men_Data"].ToString();

                pnMensagens01.Visible = true;
            }
            if (dt.Rows.Count >= 2)
            {
                mHoras = Convert.ToInt32(ConvertHoras(Convert.ToInt32(dt.Rows[1]["Minutos"].ToString())));

                lblTituloMens02.Text = dt.Rows[1]["Men_Titulo"].ToString() + ", ";
                lblHoraMens02.Text = mHoras >= 1 ? mHoras + " horas" : dt.Rows[1]["Minutos"].ToString() + " min atrás";

                if (mHoras >= 24 && mHoras <= 48) { lblHoraMens02.Text = " ontem"; }
                if (mHoras > 48) { lblHoraMens02.Text = mHoras / 24 + " dias atrás"; }

                lblPopTituloMens02.Text = dt.Rows[1]["Men_Titulo"].ToString();
                lblPopMensagem02.Text = dt.Rows[1]["Men_Descricao"].ToString();
                lblPopUpInteracao02.Text = "<b>Interação: </b>" + dt.Rows[1]["Men_Sequencia"].ToString();
                lblPopUpData02.Text = "<b>Data: </b>" + dt.Rows[1]["Men_Data"].ToString();

                pnMensagens02.Visible = true;
            }
            if (dt.Rows.Count >= 3)
            {
                mHoras = Convert.ToInt32(ConvertHoras(Convert.ToInt32(dt.Rows[2]["Minutos"].ToString())));

                lblTituloMens03.Text = dt.Rows[2]["Men_Titulo"].ToString() + ", ";
                lblHoraMens03.Text = mHoras >= 1 ? mHoras + " horas" : dt.Rows[2]["Minutos"].ToString() + " min atrás";

                if (mHoras >= 24 && mHoras <= 48) { lblHoraMens03.Text = " ontem"; }
                if (mHoras > 48) { lblHoraMens03.Text = mHoras / 24 + " dias atrás"; }

                lblPopTituloMens03.Text = dt.Rows[2]["Men_Titulo"].ToString();
                lblPopMensagem03.Text = dt.Rows[2]["Men_Descricao"].ToString();
                lblPopUpInteracao03.Text = "<b>Interação: </b>" + dt.Rows[2]["Men_Sequencia"].ToString();
                lblPopUpData03.Text = "<b>Data: </b>" + dt.Rows[2]["Men_Data"].ToString();

                pnMensagens03.Visible = true;
            }
        }
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        
    }

    protected DataTable CarregaLocalizacoes()
    {
        DataTable dt = objLocalizacao.RetornaLocalizacoes(ddlVeiculos.SelectedValue);
        if (dt != null && dt.Rows.Count > 0)        
            gvLocalizacoes.DataSource = dt;                    
        else        
            gvLocalizacoes.DataSource = null;
        
        gvLocalizacoes.DataBind();

        return dt;
    }

    protected void EscolheVeiculo()
    {
        DataTable dt = null;
        try
        {
            /*** PENDENCIA ****
             * VERIFICAR SE A DATA PARA CONSULTA ESTA EXPIRADA */

            if (ddlVeiculos.SelectedValue != "0") // caso escolheu alguma opção de veículo
            {
                txtVeiculo.Text = string.Empty;

                if (ddlVeiculos.SelectedValue != "0" && ddlVeiculos.SelectedValue != "7")
                    btnRelData.Enabled = true;
                else
                    btnRelData.Enabled = false;

                string Texto = string.Empty;
                if (ddlVeiculos.SelectedValue == "7") // Se escolheu a opção Todos os veículos
                {
                    int mAux = ddlVeiculos.Items.Count - 2;

                    txtArquivo.Text = string.Empty;
                    txtTextoPin.Text = string.Empty;
                    string[] mVetPushPin = new string[ddlVeiculos.Items.Count - 2];
                    foreach (ListItem li in ddlVeiculos.Items)
                    {
                        if (li.Value != "0" && li.Value != "7")
                        {
                            dt = objLocalizacao.NoReturnLocation(li.Value);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                txtLatitude.Text = dt.Rows[0]["His_Latitude"].ToString();
                                txtLongitude.Text = dt.Rows[0]["His_Longitude"].ToString();

                                string mColorIcone = "http://www.betablue.com.br/RastroSegue/img/RedCar.png";

                                if (dt.Rows[0]["His_Status"].ToString().TrimEnd() == "Parado") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/RedCar.png"; }
                                if (dt.Rows[0]["His_Status"].ToString().TrimEnd() == "Em movimento") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/GreenCar.png"; }
                                if (dt.Rows[0]["His_Status"].ToString().TrimEnd() == "Ligado") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/RedCar.png"; }

                                string[] mVetData = dt.Rows[0]["His_Data"].ToString().TrimEnd().Split(' ');
                                txtVeiculo.Text = ddlVeiculos.SelectedItem.ToString();

                                string mTexto = mVetData[0] + " - " + dt.Rows[0]["Vei_Placa"].ToString().TrimEnd() + " " + dt.Rows[0]["Vei_Descricao"].ToString().TrimEnd() + " " + dt.Rows[0]["His_Hora"].ToString().TrimEnd() + " - " + dt.Rows[0]["His_Speed"].ToString().TrimEnd().Replace(',', '.') + "Km/h";

                                txtArquivo.Text = txtArquivo.Text + dt.Rows[0]["His_Latitude"].ToString().TrimEnd() + "," + dt.Rows[0]["His_Longitude"].ToString().TrimEnd() + "," + mColorIcone + "," + mTexto;
                                if (mAux > 0)
                                    txtArquivo.Text = txtArquivo.Text + ",";

                                mAux--;
                            }
                        }
                    }
                    txtZoom.Text = (ddlVeiculos.Items.Count - 2).ToString();
                    ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMapZoomMulti();</script>");
                }
                else // caso escolheu um veículo
                {
                    CarregaAvisos();

                    dt = objLocalizacao.RetornaLocalizacoes(ddlVeiculos.SelectedValue);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt != null && dt.Rows.Count > 0)
                            gvLocalizacoes.DataSource = dt;
                        else
                            gvLocalizacoes.DataSource = null;
                        gvLocalizacoes.DataBind();

                        TimeSpan date = Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(dt.Rows[0]["His_Data"].ToString());

                        int totalDias = date.Days;

                        string[] mVetVelocidade = dt.Rows[0]["His_Speed"].ToString().Replace(',', '.').Split('.');
                        int mVelocidade = Convert.ToInt32(mVetVelocidade[0]);

                        string mColorIcone = "http://www.betablue.com.br/RastroSegue/img/RedCar.png";
                        if (dt.Rows[0]["His_Status"].ToString().TrimEnd() == "Parado") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/RedCar.png"; }
                        if (dt.Rows[0]["His_Status"].ToString().TrimEnd() == "Em movimento") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/GreenCar.png"; }
                        if (dt.Rows[0]["His_Status"].ToString().TrimEnd() == "Ligado") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/RedCar.png"; }

                        txtIconeColor.Text = mColorIcone;

                        txtLatitude.Text = dt.Rows[0]["His_Latitude"].ToString().TrimEnd().ToString();
                        txtLongitude.Text = dt.Rows[0]["His_Longitude"].ToString().TrimEnd().ToString();
                        txtZoom.Text = "17";

                        string[] mVetData = dt.Rows[0]["His_Data"].ToString().TrimEnd().Split(' ');
                        txtVeiculo.Text = ddlVeiculos.SelectedItem.ToString();                        

                        txtTextoPin.Text = "Localização em " + mVetData[0] + " as " + dt.Rows[0]["His_Hora"].ToString().TrimEnd() + " "
                                  + "Endereço aproximado: " + objLocalizacao.ReturnAdress(dt.Rows[0]["His_Latitude"].ToString().TrimEnd(), dt.Rows[0]["His_Longitude"].ToString().TrimEnd()) + " "
                                  + "Velocidade: " + dt.Rows[0]["His_Speed"].ToString().Replace(",", ".") + " - Status: " + dt.Rows[0]["His_Status"].ToString().TrimEnd();

                        // Mensagens na tela caso o veículo não retorna localizações há alguns dias
                        if (date.Days >= 4 && (dt.Rows[0]["Vei_24Horas"].ToString() == "S" || dt.Rows[0]["Vei_24Horas"].ToString() == "A"))
                        {
                            ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMapZoom();alert('O localizador deste veículo não retorna localizações a cerca de " + date.Days + " dias, por favor verifique se o aparelho está ligado corretamente!');</script>");
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMapZoom();</script>");
                        }
                    }
                    else
                    {
                        gvLocalizacoes.DataSource = null;
                        gvLocalizacoes.DataBind();
                        ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMap();</script>");
                    }
                }
                TimerLocalizacoes.Enabled = true;
            }
            else
            {
                gvLocalizacoes.DataSource = null;
                gvLocalizacoes.DataBind();
                ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMap();</script>");
            }
        }
        catch (Exception err)
        { ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Erro: " + err) + "');</script>"); }        
    }
    protected void gvLocalizacoes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvLocalizacoes.PageIndex = e.NewPageIndex;
        DataTable dt = objLocalizacao.RetornaLocalizacoes(ddlVeiculos.SelectedValue);
        gvLocalizacoes.DataSource = dt;
        gvLocalizacoes.DataBind();
    }
    protected void gvLocalizacoes_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            // Plotar uma localizações escolhida pelo usuário
            DataTable dt = objLocalizacao.RetornaLocalizacaoDesejada(gvLocalizacoes.SelectedRow.Cells[2].Text, gvLocalizacoes.SelectedRow.Cells[4].Text, gvLocalizacoes.SelectedRow.Cells[5].Text.TrimEnd());
            if (dt != null && dt.Rows.Count > 0)
            {                
                string mColorIcone = "http://www.betablue.com.br/RastroSegue/img/RedCar.png";
                string[] mVetVelocidade = dt.Rows[0]["His_Speed"].ToString().Replace(",", ".").Split('.');
                int mVelocidade = Convert.ToInt32(mVetVelocidade[0]);

                mColorIcone = "http://www.betablue.com.br/RastroSegue/img/RedCar.png";
                if (dt.Rows[0]["His_Status"].ToString().TrimEnd() == "Parado") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/RedCar.png"; }
                if (dt.Rows[0]["His_Status"].ToString().TrimEnd() == "Em movimento") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/GreenCar.png"; }
                if (dt.Rows[0]["His_Status"].ToString().TrimEnd() == "Ligado") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/RedCar.png"; }

                txtIconeColor.Text = mColorIcone;

                txtLatitude.Text = dt.Rows[0]["His_Latitude"].ToString().TrimEnd().ToString();
                txtLongitude.Text = dt.Rows[0]["His_Longitude"].ToString().TrimEnd().ToString();
                txtZoom.Text = "17";

                string[] mVetData = dt.Rows[0]["His_Data"].ToString().TrimEnd().Split(' ');
                txtVeiculo.Text = ddlVeiculos.SelectedItem.ToString();

                txtTextoPin.Text = "Localização em " + mVetData[0] + " as " + dt.Rows[0]["His_Hora"].ToString().TrimEnd() + " - "
                              + "Velocidade: " + dt.Rows[0]["His_Speed"].ToString() + " - Status: " + dt.Rows[0]["His_Status"].ToString().TrimEnd() + " - "
                              + objLocalizacao.ReturnAdress(dt.Rows[0]["His_Latitude"].ToString(), dt.Rows[0]["His_Longitude"].ToString());

                ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMapZoom();</script>");                
            }
        }
        catch (Exception err)
        { }
    }
    protected void ddlVeiculos_SelectedIndexChanged(object sender, EventArgs e)
    {
        EscolheVeiculo();                
    }
    protected void ConfirmaMensagemLida(string mCodMensagem)
    {
        string mComandoSQL = "|Update Tab_Mensagens Set "
                           + "Men_Status = 'L' "
                           + "Where Men_Sequencia = " + mCodMensagem;
        if (objMensagens.Gravar(mComandoSQL) >= 1)
        { }
    }
    protected void btnFecharPopUp01_Click(object sender, EventArgs e)
    {
        string[] mVet = lblPopUpInteracao01.Text.Split(':');
        ConfirmaMensagemLida(mVet[1].Substring(5, mVet[1].Length - 5));
        CarregaMensagens();
    }
    protected void btnFecharPopUp02_Click(object sender, EventArgs e)
    {
        string[] mVet = lblPopUpInteracao02.Text.Split(':');
        ConfirmaMensagemLida(mVet[1].Substring(5, mVet[1].Length - 5));
        CarregaMensagens();
    }
    protected void btnFecharPopUp03_Click(object sender, EventArgs e)
    {
        string[] mVet = lblPopUpInteracao03.Text.Split(':');
        ConfirmaMensagemLida(mVet[1].Substring(5, mVet[1].Length - 5));
        CarregaMensagens();
    }
    protected void btnCarregaLocal_Click(object sender, ImageClickEventArgs e)
    {        
        pnRelData.Visible = false;
        CarregaLocalizacoes();
    }
    protected void btnLocalizar_Click(object sender, ImageClickEventArgs e)
    {
        pnRelData.Visible = false;     
        CarregaLocalizacoes();
    }
    protected void btnLocaliza24Horas_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            DataTable dt = objVeiculos.VeiculoLocalizacao24Horas(Convert.ToInt32(ddlVeiculos.SelectedValue));
            if (dt != null && dt.Rows.Count > 0)
            {
                DateTime data1;
                DateTime data2;
                data1 = DateTime.Parse(DateTime.Now.ToShortDateString());
                data2 = DateTime.Parse(dt.Rows[0]["Sal_DtFinal"].ToString());
                if (data2.CompareTo(data1) >= 0)
                {
                    string mComando = string.Empty;
                    if (dt.Rows[0]["Vei_24Horas"].ToString() == "N")
                    {
                        if (dt.Rows[0]["Mod_Codigo"].ToString() == "2" || dt.Rows[0]["Mod_Codigo"].ToString() == "3") // Família TK, Gravar o status com Ativado e solicitar a localização infinita
                        {
                            if (dt.Rows[0]["Chip_Operadora"].ToString() == "VIVO")
                                objVeiculos.NumTelChip = "0" + dt.Rows[0]["Chip_NumTel"].ToString().Substring(3, 12);
                            else
                                objVeiculos.NumTelChip = dt.Rows[0]["Chip_NumTel"].ToString();

                            mComando = "|Update Tab_Veiculos Set "
                                     + "Vei_24Horas = 'A' "
                                     + "Where Vei_Codigo = " + dt.Rows[0]["Vei_Codigo"].ToString();
                            mComando = mComando + "|Insert Into Tab_SolicitacoesHaver (Vei_NumTelChip, SoH_Status, SoH_Data, SoH_Hora, SoH_Mensagem, Ser_Codigo) Values ('"
                                                + objVeiculos.NumTelChip + "', 'Pendente', '" + DateTime.Now.ToShortDateString() + "', '" + DateTime.Now.ToLongTimeString() + "', '"
                                                + "fix300s***n" + dt.Rows[0]["Loc_Senha"].ToString() + "', '" + dt.Rows[0]["Ser_Codigo"].ToString() + "')";
                        }

                        if (dt.Rows[0]["Mod_Codigo"].ToString() == "6") // Família TLT
                        {
                            if (dt.Rows[0]["Chip_Operadora"].ToString() == "VIVO")
                                objVeiculos.NumTelChip = "0" + dt.Rows[0]["Chip_NumTel"].ToString().Substring(3, 12);
                            else
                                objVeiculos.NumTelChip = dt.Rows[0]["Chip_NumTel"].ToString();

                            mComando = "|Update Tab_Veiculos Set "
                                     + "Vei_24Horas = 'A' "
                                     + "Where Vei_Codigo = " + dt.Rows[0]["Vei_Codigo"].ToString();
                            mComando = mComando + "|Insert Into Tab_SolicitacoesHaver (Vei_NumTelChip, SoH_Status, SoH_Data, SoH_Hora, SoH_Mensagem, Ser_Codigo) Values ('"
                                                + objVeiculos.NumTelChip + "', 'Pendente', '" + DateTime.Now.ToShortDateString() + "', '" + DateTime.Now.ToLongTimeString() + "', '"
                                                + "710" + dt.Rows[0]["Loc_Senha"].ToString() + "', '" + dt.Rows[0]["Ser_Codigo"].ToString() + "')";
                        }

                        if (dt.Rows[0]["Mod_Codigo"].ToString() == "7") // TK 102a
                        {
                            if (dt.Rows[0]["Chip_Operadora"].ToString() == "VIVO")
                                objVeiculos.NumTelChip = "0" + dt.Rows[0]["Chip_NumTel"].ToString().Substring(3, 12);
                            else
                                objVeiculos.NumTelChip = dt.Rows[0]["Chip_NumTel"].ToString();

                            mComando = "|Update Tab_Veiculos Set "
                                     + "Vei_24Horas = 'A' "
                                     + "Where Vei_Codigo = " + dt.Rows[0]["Vei_Codigo"].ToString();
                            mComando = mComando + "|Insert Into Tab_SolicitacoesHaver (Vei_NumTelChip, SoH_Status, SoH_Data, SoH_Hora, SoH_Mensagem, Ser_Codigo) Values ('"
                                                + objVeiculos.NumTelChip + "', 'Pendente', '" + DateTime.Now.ToShortDateString() + "', '" + DateTime.Now.ToLongTimeString() + "', '"
                                                + "t300s***n" + dt.Rows[0]["Loc_Senha"].ToString() + "', '" + dt.Rows[0]["Ser_Codigo"].ToString() + "')";
                        }

                        if (dt.Rows[0]["Mod_Codigo"].ToString() == "4") // Familia GT
                        {
                            mComando = "|Update Tab_Veiculos Set "
                                     + "Vei_24Horas = 'S' "
                                     + "Where Vei_Codigo = " + dt.Rows[0]["Vei_Codigo"].ToString();
                        }

                        if (dt.Rows[0]["Mod_Codigo"].ToString() == "8") // TK 102c
                        {
                            if (dt.Rows[0]["Chip_Operadora"].ToString() == "VIVO")
                                objVeiculos.NumTelChip = "0" + dt.Rows[0]["Chip_NumTel"].ToString().Substring(3, 12);
                            else
                                objVeiculos.NumTelChip = dt.Rows[0]["Chip_NumTel"].ToString();

                            mComando = "|Update Tab_Veiculos Set "
                                     + "Vei_24Horas = 'A' "
                                     + "Where Vei_Codigo = " + dt.Rows[0]["Vei_Codigo"].ToString();
                            mComando = mComando + "|Insert Into Tab_SolicitacoesHaver (Vei_NumTelChip, SoH_Status, SoH_Data, SoH_Hora, SoH_Mensagem, Ser_Codigo) Values ('"
                                                + objVeiculos.NumTelChip + "', 'Pendente', '" + DateTime.Now.ToShortDateString() + "', '" + DateTime.Now.ToLongTimeString() + "', '"
                                                + "#at#300#sum#0#" + dt.Rows[0]["Loc_Senha"].ToString() + "', '" + dt.Rows[0]["Ser_Codigo"].ToString() + "')";
                        }

                        if (objVeiculos.Gravar(mComando) == 1)
                        {
                            ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Localização 24 horas habilitada!") + "');</script>");
                            btnParaLocalizacao.Visible = true;
                            btnLocaliza24Horas.Visible = false;
                        }
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Data expirada!") + "');</script>");
                }                
            }
        }
        catch (Exception err)
        { ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Erro: " + err) + "');</script>"); }
    }
    protected void btnParaLocalizacao_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            DataTable dt = objVeiculos.VeiculoLocalizacao24Horas(Convert.ToInt32(ddlVeiculos.SelectedValue));
            if (dt != null && dt.Rows.Count > 0)
            {
                string mComando = string.Empty;
                if (dt.Rows[0]["Vei_24Horas"].ToString() == "S" || dt.Rows[0]["Vei_24Horas"].ToString() == "A")
                {
                    mComando = "|Update Tab_Veiculos Set "
                             + "Vei_24Horas = 'N' "
                             + "Where Vei_Codigo = " + dt.Rows[0]["Vei_Codigo"].ToString();
                    if (dt.Rows[0]["Mod_Codigo"].ToString() == "2" || dt.Rows[0]["Mod_Codigo"].ToString() == "3" || dt.Rows[0]["Mod_Codigo"].ToString() == "7") // Família TK, envio o comando para a localização 24 horas                    
                    {
                        if (dt.Rows[0]["Chip_Operadora"].ToString() == "VIVO")
                            objVeiculos.NumTelChip = "0" + dt.Rows[0]["Chip_NumTel"].ToString().Substring(3, 12);
                        else
                            objVeiculos.NumTelChip = dt.Rows[0]["Chip_NumTel"].ToString();

                        mComando = mComando + "|Insert Into Tab_SolicitacoesHaver (Vei_NumTelChip, SoH_Status, SoH_Data, SoH_Hora, SoH_Mensagem, Ser_Codigo) Values ('"
                                           + objVeiculos.NumTelChip + "', 'Pendente', '" + DateTime.Now.ToShortDateString() + "', '" + DateTime.Now.ToLongTimeString() + "', '"
                                           + "nofix" + dt.Rows[0]["Loc_Senha"].ToString() + "', '" + dt.Rows[0]["Ser_Codigo"].ToString() + "')";
                    }
                    if (dt.Rows[0]["Mod_Codigo"].ToString() == "6") // Familia TLT         
                    {
                        if (dt.Rows[0]["Chip_Operadora"].ToString() == "VIVO")
                            objVeiculos.NumTelChip = "0" + dt.Rows[0]["Chip_NumTel"].ToString().Substring(3, 12);
                        else
                            objVeiculos.NumTelChip = dt.Rows[0]["Chip_NumTel"].ToString();

                        mComando = mComando + "|Insert Into Tab_SolicitacoesHaver (Vei_NumTelChip, SoH_Status, SoH_Data, SoH_Hora, SoH_Mensagem, Ser_Codigo) Values ('"
                                           + objVeiculos.NumTelChip + "', 'Pendente', '" + DateTime.Now.ToShortDateString() + "', '" + DateTime.Now.ToLongTimeString() + "', '"
                                           + "700" + dt.Rows[0]["Loc_Senha"].ToString() + "', '" + dt.Rows[0]["Ser_Codigo"].ToString() + "')";
                    }
                    if (objVeiculos.Gravar(mComando) == 1)
                    {
                        ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Localização 24 horas desabilitada!") + "');</script>");
                        btnLocaliza24Horas.Visible = true;
                        btnParaLocalizacao.Visible = false;
                    }
                }                
            }
        }
        catch (Exception err)
        { ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Erro: " + err) + "');</script>"); }
    }
    protected void HabilitaRelData()
    {
        ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMap();</script>");

        gvLocalizacoes.DataSource = null;
        gvLocalizacoes.DataBind();

        pnRelData.Visible = true;

        Session["mData"] = false;

        txtDtFinal.Text = DateTime.Now.ToShortDateString();
        txtDtFinal.Focus();
    }
    protected void btnRelData_Click(object sender, ImageClickEventArgs e)
    {
        HabilitaRelData();
    }
    protected void btnRelViagens_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("Rel-Trafego.aspx");
    }
    protected void btnRelPercurso_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("Rel-Percurso.aspx");
    }
    protected void btnPagamento_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("Doc-Faturas.aspx");
    }
    protected void btnContato_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("Doc-Contato.aspx");
    }
    protected void btnExibeRelData_Click(object sender, EventArgs e)
    {
        try
        {
            if (ddlVeiculos.SelectedValue == "0")
            {
                ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Escolha o veículo.") + "');</script>");
                ddlVeiculos.Focus();
            }
            else
            {
                if (txtDtFinal.Text != "" && ddlVeiculos.SelectedValue != "0")
                {
                    txtArquivo.Text = string.Empty;
                    DataTable dt = objLocalizacao.RetornaLocalizacoes(txtDtFinal.Text, ddlVeiculos.SelectedValue);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        txtTextoPin.Text = string.Empty;
                        string[] mVetPushPin = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string mColorIcone = "#00ff00";
                            string[] mVetVelocidade = dt.Rows[i]["His_Speed"].ToString().Replace(',', '.').Split('.');
                            int mVelocidade = Convert.ToInt32(mVetVelocidade[0]);

                            mColorIcone = "http://www.betablue.com.br/RastroSegue/img/GreenCar.png";
                            if (dt.Rows[i]["His_Status"].ToString().TrimEnd() == "Parado") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/RedCar.png"; }
                            if (dt.Rows[i]["His_Status"].ToString().TrimEnd() == "Em movimento") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/GreenCar.png"; }
                            if (dt.Rows[i]["His_Status"].ToString().TrimEnd() == "Ligado") { mColorIcone = "http://www.betablue.com.br/RastroSegue/img/GreenCar.png"; }                            

                            string[] mVetData = dt.Rows[0]["His_Data"].ToString().TrimEnd().Split(' ');
                            txtVeiculo.Text = ddlVeiculos.SelectedItem.ToString();

                            string mTexto = mVetData[0] + " - " + dt.Rows[i]["His_Hora"].ToString().TrimEnd() + " - " + dt.Rows[i]["His_Speed"].ToString().TrimEnd().Replace(',', '.') + "Km/h";

                            mVetPushPin[i] = "'lat': " + dt.Rows[i]["His_Latitude"].ToString().TrimEnd().ToString() + ", 'lng': " + dt.Rows[i]["His_Longitude"].ToString().TrimEnd().ToString() + ", 'title': '" + ddlVeiculos.SelectedItem.ToString().TrimEnd() + "', 'description': " + mTexto + "';";
                            txtTextoPin.Text = txtTextoPin.Text + mVetPushPin[i];

                            txtArquivo.Text = txtArquivo.Text + dt.Rows[i]["His_Latitude"].ToString().TrimEnd() + "," + dt.Rows[i]["His_Longitude"].ToString().TrimEnd() + "," + mColorIcone + "," + mTexto;
                            if (i < dt.Rows.Count)
                                txtArquivo.Text = txtArquivo.Text + ",";
                        }
                        txtLatitude.Text = dt.Rows[0]["His_Latitude"].ToString().TrimEnd();
                        txtLongitude.Text = dt.Rows[0]["His_Longitude"].ToString().TrimEnd();
                        txtZoom.Text = dt.Rows.Count.ToString();
                        ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMapZoomMulti();</script>");

                        gvLocalizacoes.DataSource = null;
                        gvLocalizacoes.DataBind();
                        TimerLocalizacoes.Enabled = false;
                    }
                    else
                    { ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Nenhuma informação cadastrada para essa data e veículo!") + "');GetMap();</script>"); }
                }
                else
                { ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Escolha o veículo e data!") + "');</script>"); }
            }
        }
        catch (Exception err)
        { ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>alert('" + ("Erro: " + err) + "');</script>"); }
    }
    protected void TimerLocalizacoes_Tick(object sender, EventArgs e)
    {        
        EscolheVeiculo();        
    }

    protected void lybRelData_Click(object sender, EventArgs e)
    {
        HabilitaRelData();
    }

    protected void btnFrota_Click(object sender, ImageClickEventArgs e)
    {     
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "redirect", "window.open('../RastroSegueFrota/System/Principal2.aspx?mCodUsuario=" + Session["mCodUsuario"].ToString() + "');", true);     
    }

    protected void chAtualizacaoPagina_CheckedChanged(object sender, EventArgs e)
    {
        if (chAtualizacaoPagina.Checked)
            TimerLocalizacoes.Enabled = true;
        else
            TimerLocalizacoes.Enabled = false;

        CarregaLocalizacoes();
        ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMap();</script>");
    }

    protected void btnRefresh_Click(object sender, ImageClickEventArgs e)
    {
        CarregaLocalizacoes();
        EscolheVeiculo();
        ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMapZoom();</script>");

        TimerLocalizacoes.Enabled = true;
        btnRefresh.Visible = false;
        btnRefreshNo.Visible = true;
    }

    protected void btnRefreshNo_Click(object sender, ImageClickEventArgs e)
    {
        CarregaLocalizacoes();
        EscolheVeiculo();
        ClientScript.RegisterStartupScript(typeof(Page), "alert", "<script language=JavaScript>GetMapZoom();</script>");

        TimerLocalizacoes.Enabled = false;
        btnRefresh.Visible = true;
        btnRefreshNo.Visible = false;
    }
}