<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrincipalOSM.aspx.cs" Inherits="System02_Principal___OSM" validateRequest="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<%@ Register Src="WUCLogin.ascx" TagPrefix="uc1" TagName="WUCLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>.:: RastroSegue ::.</title>

    <meta name="author" content="FP Soluções Tecnológicas LTDA" />
    <meta name="description" content="Plataforma online para auxilio na localização de veículos" />
    <meta name="keywords" content="rastreamento, localizar, veiculo, localização de veículos" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="css2/bootstrap.min.css" rel="stylesheet" />
    <link href="css2/bootstrap-responsive.min.css" rel="stylesheet" />
    <link href="css2/style.css" rel="stylesheet" />
    <link href="css2/style-responsive.css" rel="stylesheet" />

    <!--<script src="http://www.openlayers.org/api/OpenLayers.js"></script>-->
    <script src="https://openlayers.org/api/OpenLayers.js"></script>

    <script>
        function GetMap() {
            var lat = -23.405875;
            var lon = -51.936458;
            var zoom = 3;

            var fromProjection = new OpenLayers.Projection("EPSG:4326");   // Transform from WGS 1984
            var toProjection = new OpenLayers.Projection("EPSG:900913"); // to Spherical Mercator Projection
            var position = new OpenLayers.LonLat(lon, lat).transform(fromProjection, toProjection);

            map = new OpenLayers.Map("mapdiv");
            var mapnik = new OpenLayers.Layer.OSM();
            map.addLayer(mapnik);

            map.setCenter(position, zoom);
        }
        function GetMapZoom() {            
            var lat = document.getElementById("txtLatitude").value;
            var lon = document.getElementById("txtLongitude").value;

            map = new OpenLayers.Map("mapdiv");
            map.addLayer(new OpenLayers.Layer.OSM());

            var fromProjection = new OpenLayers.Projection("EPSG:4326");   // Transform from WGS 1984
            var toProjection = new OpenLayers.Projection("EPSG:900913"); // to Spherical Mercator Projection

            var lonLat = new OpenLayers.LonLat(lon, lat).transform(fromProjection, toProjection);

            var zoom = 17; 
            map.setCenter(lonLat, zoom);

            var vectorLayer = new OpenLayers.Layer.Vector("Overlay");

            var position = new OpenLayers.LonLat(lat, lon).transform(fromProjection, toProjection);

            var feature = new OpenLayers.Feature.Vector(
                    new OpenLayers.Geometry.Point(lon, lat).transform(fromProjection, toProjection),
                    { description: document.getElementById("txtTextoPin").value },
                    { externalGraphic: document.getElementById("txtIconeColor").value, graphicHeight: 35, graphicWidth: 31, graphicXOffset: -22, graphicYOffset: -35 }
                );
            vectorLayer.addFeatures(feature);

            map.addLayer(vectorLayer);

            var controls = {
                selector: new OpenLayers.Control.SelectFeature(vectorLayer, { onSelect: createPopup, onUnselect: destroyPopup })
            };

            function createPopup(feature) {
                feature.popup = new OpenLayers.Popup.FramedCloud("pop",
                    feature.geometry.getBounds().getCenterLonLat(),
                    null,
                    '<div class="markerContent">' + feature.attributes.description + '</div>',
                    null,
                    true,
                    function () { controls['selector'].unselectAll(); }
                );

                map.addPopup(feature.popup);
            }

            function destroyPopup(feature) {
                feature.popup.destroy();
                feature.popup = null;
            }

            map.addControl(controls['selector']);
            controls['selector'].activate();
        }
        function GetMapZoomMulti() {
            var lat = document.getElementById("txtLatitude").value;
            var lon = document.getElementById("txtLongitude").value;

            map = new OpenLayers.Map("mapdiv");
            map.addLayer(new OpenLayers.Layer.OSM());

            var epsg4326 = new OpenLayers.Projection("EPSG:4326"); //WGS 1984 projection
            var projectTo = map.getProjectionObject(); //The map projection (Spherical Mercator)

            var lonLat = new OpenLayers.LonLat(lon, lat).transform(epsg4326, projectTo);

            var zoom = 7;
            map.setCenter(lonLat, zoom);

            var vectorLayer = new OpenLayers.Layer.Vector("Overlay");

            var arquivoPos = document.getElementById("txtArquivo").value.split(",");

            for (var cont = 0; cont < arquivoPos.length; cont = cont + 4) {
                var feature = new OpenLayers.Feature.Vector(
                        new OpenLayers.Geometry.Point(arquivoPos[cont+1], arquivoPos[cont+0]).transform(epsg4326, projectTo),
                        { description: arquivoPos[cont+3] },
                        { externalGraphic: arquivoPos[cont + 2], graphicHeight: 25, graphicWidth: 21, graphicXOffset: -12, graphicYOffset: -25 }
                    );
                vectorLayer.addFeatures(feature);
            }            

            map.addLayer(vectorLayer);

            var controls = {
                selector: new OpenLayers.Control.SelectFeature(vectorLayer, { onSelect: createPopup, onUnselect: destroyPopup })
            };

            function createPopup(feature) {
                feature.popup = new OpenLayers.Popup.FramedCloud("pop",
                    feature.geometry.getBounds().getCenterLonLat(),
                    null,
                    '<div class="markerContent">' + feature.attributes.description + '</div>',
                    null,
                    true,
                    function () { controls['selector'].unselectAll(); }
                );

                map.addPopup(feature.popup);
            }

            function destroyPopup(feature) {
                feature.popup.destroy();
                feature.popup = null;
            }

            map.addControl(controls['selector']);
            controls['selector'].activate();
        }
    </script>
    
    <!--<link rel="shortcut icon" href="img/favicon.ico" />

    <link href="cssPopUp/assets/docs.css" rel="stylesheet" />
    <script src="cssPopUp/lib/sweet-alert.js"></script>
    <link rel="stylesheet" href="cssPopUp/lib/sweet-alert.css" />

    <link rel="stylesheet" href="css2/reveal.css" />
    <script type="text/javascript" src="http://code.jquery.com/jquery-1.6.min.js"></script>
    <script type="text/javascript" src="css2/jquery.reveal.js"></script>
        -->
    
</head>
<body>
    <!--<body onload="load()" onunload="GUnload()">-->
    <uc1:WUCLogin runat="server" ID="WUCLogin" />
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <!-- start: Header -->
        <div class="navbar">
            <div class="navbar-inner">
                <div class="container-fluid">
                    <a class="btn btn-navbar" data-toggle="collapse" data-target=".top-nav.nav-collapse,.sidebar-nav.nav-collapse">
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </a>
                    <a class="brand" href="PrincipalOSM.aspx"><span>RASTROSEGUE</span></a>

                    <!-- start: Header Menu -->
                    <div class="nav-no-collapse header-nav">
                        <ul class="nav pull-right">
                            <li class="dropdown hidden-phone">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <a class="btn dropdown-toggle" data-toggle="dropdown" href="#">
                                            <i class="icon-bell"></i>
                                            <span class="badge red">
                                                <asp:Label ID="lblTotalAvisosPendentes" runat="server" Text=""></asp:Label></span>
                                        </a>

                                        <ul class="dropdown-menu notifications">
                                            <li class="dropdown-menu-title">
                                                <span>
                                                    <asp:Label ID="lblTotalAvisos" runat="server" Text=""></asp:Label></span>
                                                <a href="#"><i class="icon-repeat"></i></a>
                                            </li>
                                            <asp:Panel ID="pnAviso01" runat="server">
                                                <li>
                                                    <a>
                                                        <span class="icon green"><i class="icon-comment-alt"></i></span>
                                                        <span class="message">
                                                            <asp:Label ID="lblAviso01" runat="server" Text=""></asp:Label>
                                                            <span class="time">
                                                                <asp:Label ID="lblAvisoMin01" runat="server" Text=""></asp:Label></span>                                                                                            
                                                    </a>
                                                </li>
                                            </asp:Panel>
                                            <asp:Panel ID="pnAviso02" runat="server">
                                                <li>
                                                    <a>
                                                        <span class="icon green"><i class="icon-comment-alt"></i></span>
                                                        <span class="message">
                                                            <asp:Label ID="lblAviso02" runat="server" Text=""></asp:Label></span>
                                                        <span class="time">
                                                            <asp:Label ID="lblAvisoMin02" runat="server" Text=""></asp:Label></span>
                                                    </a>
                                                </li>
                                            </asp:Panel>
                                            <asp:Panel ID="pnAviso03" runat="server">
                                                <li>
                                                    <a>
                                                        <span class="icon green"><i class="icon-comment-alt"></i></span>
                                                        <span class="message">
                                                            <asp:Label ID="lblAviso03" runat="server" Text=""></asp:Label></span>
                                                        <span class="time">
                                                            <asp:Label ID="lblAvisoMin03" runat="server" Text=""></asp:Label></span>
                                                    </a>
                                                </li>
                                            </asp:Panel>
                                            <asp:Panel ID="pnAviso04" runat="server">
                                                <li>
                                                    <a>
                                                        <span class="icon green"><i class="icon-comment-alt"></i></span>
                                                        <span class="message">
                                                            <asp:Label ID="lblAviso04" runat="server" Text=""></asp:Label></span>
                                                        <span class="time">
                                                            <asp:Label ID="lblAvisoMin04" runat="server" Text=""></asp:Label></span>
                                                    </a>
                                                </li>
                                            </asp:Panel>

                                            <li class="dropdown-menu-sub-footer">
                                                <asp:HyperLink ID="hplTodosAvisos" runat="server">Ver todos os avisos</asp:HyperLink>
                                            </li>
                                        </ul>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </li>

                            <li class="dropdown hidden-phone">
                                <a class="btn dropdown-toggle" data-toggle="dropdown" href="#">
                                    <i class="icon-envelope"></i>
                                    <span class="badge red">
                                        <asp:Label ID="lblTotalNumMens" runat="server" Text=""></asp:Label>
                                    </span>
                                </a>
                                <ul class="dropdown-menu messages">
                                    <li class="dropdown-menu-title">
                                        <span>
                                            <asp:Label ID="lblTotalMens" runat="server" Text=""></asp:Label></span>
                                        <a href="#refresh"><i class="icon-repeat"></i></a>
                                    </li>
                                    <asp:Panel ID="pnMensagens01" runat="server">
                                        <li>
                                            <div style="float: left; width: auto;">
                                                <span class="avatar" />
                                            </div>
                                            <div style="float: left; width: auto;">
                                                <span class="header">
                                                    <asp:HyperLink ID="hplMensagem01" runat="server" class="big-link" data-reveal-id="myModal01">
                                                        <span class="from">
                                                            <asp:Label ID="lblTituloMens01" runat="server" Text=""></asp:Label>

                                                            <asp:Label ID="lblHoraMens01" runat="server" Text=""></asp:Label>
                                                        </span>
                                                    </asp:HyperLink>
                                                </span>
                                            </div>                                            
                                        </li>
                                    </asp:Panel>
                                    <asp:Panel ID="pnMensagens02" runat="server">
                                        <br />
                                        <br />
                                        <li>
                                            <div style="float: left; width: auto;">
                                                <span class="avatar"></span>
                                            </div>
                                            <div style="float: left; width: auto;">
                                                <span class="header">
                                                    <asp:HyperLink ID="hplMensagem02" runat="server" class="big-link" data-reveal-id="myModal02">
                                                        <span class="from">
                                                            <asp:Label ID="lblTituloMens02" runat="server" Text=""></asp:Label>

                                                            <asp:Label ID="lblHoraMens02" runat="server" Text=""></asp:Label>
                                                        </span>
                                                    </asp:HyperLink>
                                                </span>
                                            </div>                                            
                                        </li>
                                    </asp:Panel>
                                    <asp:Panel ID="pnMensagens03" runat="server">
                                        <br />
                                        <br />
                                        <li>
                                            <div style="float: left; width: auto;">
                                                <span class="avatar"></span>
                                            </div>
                                            <div style="float: left; width: auto;">
                                                <span class="header">
                                                    <asp:HyperLink ID="hplMensagem03" runat="server" class="big-link" data-reveal-id="myModal03">
                                                        <span class="from">
                                                            <asp:Label ID="lblTituloMens03" runat="server" Text=""></asp:Label>
                                                  
                                                            <asp:Label ID="lblHoraMens03" runat="server" Text=""></asp:Label>
                                                        </span>
                                                    </asp:HyperLink>
                                                </span>
                                            </div>
                                        </li>
                                    </asp:Panel>
                                    <li>
                                        <a href="Doc-Mensagens.aspx" class="dropdown-menu-sub-footer">Ver todas as mensagens</a>
                                    </li>
                                </ul>
                            </li>

                            <li class="dropdown hidden-phone">
                                <a class="btn dropdown-toggle" data-toggle="dropdown">
                                    <i class="halflings-icon white user"></i>
                                    <asp:Label ID="lblLoginUsuario" runat="server" Text=""></asp:Label>

                                    <span class="caret"></span>
                                </a>
                                <ul class="dropdown-menu message">
                                    <li class="dropdown-menu-title">
                                        <span>
                                            <asp:Label ID="lblNomeLogin" runat="server" Text=""></asp:Label></span>
                                    </li>
                                    <li><i class="halflings-icon off"></i>
                                        <asp:Label ID="lblNomeCompleto" ForeColor="Black" runat="server" Text=""></asp:Label></li>
                                    <li><a href="Logout.aspx"><i class="halflings-icon off"></i>Logout</a></li>
                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <div class="container-fluid-full">
            <div class="row-fluid">
                <div id="sidebar-left" class="span2">
                    <div class="nav-collapse sidebar-nav">
                        <ul class="nav nav-tabs nav-stacked main-menu">
                            <li><a href="PrincipalOSM.aspx"><i class="icon-bar-chart"></i><span class="hidden-tablet">Principal</span></a></li>
                            <li><a href="Doc-PainelControle.aspx"><i class="icon-align-justify"></i><span class="hidden-tablet">Painel de controle</span></a></li>
                            <li>
                                <a class="dropmenu" href="#"><i class="icon-folder-close-alt"></i><span class="hidden-tablet">Relatórios</span><span class="label label-important"> 3 </span></a>
                                <ul>
                                    <li><asp:LinkButton ID="lybRelData" runat="server" class="submenu" OnClick="lybRelData_Click"><i class="icon-file-alt"></i><span class="hidden-tablet">Por data</span></asp:LinkButton></li>
                                    
                                    <li><a class="submenu" href="Rel-Trafego.aspx"><i class="icon-file-alt"></i><span class="hidden-tablet">Trafego</span></a></li>
                                    
                                </ul>
                            </li>
                            <li><a href="Doc-Faturas.aspx"><i class="icon-calendar"></i><span class="hidden-tablet">Faturas</span></a></li>
                            <li><a href="Doc-Contato.aspx"><i class="icon-list-alt"></i><span class="hidden-tablet">Contato</span></a></li>
                            <li><a href="Logout.aspx"><i class="icon-lock"></i><span class="hidden-tablet">Logout</span></a></li>
                        </ul>
                    </div>
                </div>
                <div id="content" class="span10">
                    <ul class="breadcrumb">
                        <li>
                            <i class="icon-home"></i>
                            <a href="Principal2.aspx">Principal</a>
                            <i class="icon-angle-right"></i>
                        </li>
                    </ul>

                    <div class="row-fluid">
                        
                                <asp:ObjectDataSource ID="ODSVeiculos" runat="server" SelectMethod="RetornaVeiculos" TypeName="Veiculos">
                                    <SelectParameters>
                                        <asp:SessionParameter DefaultValue="" Name="mLogin" SessionField="mCodCliente" Type="String" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                                <asp:ObjectDataSource ID="ODSVeiculosCere" runat="server" SelectMethod="RetornaVeiculosCere" TypeName="Veiculos">
                                    <SelectParameters>
                                        <asp:SessionParameter DefaultValue="" Name="mLogin" SessionField="mCodCliente" Type="String" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                                <asp:Timer ID="TimerLocalizacoes" runat="server" OnTick="TimerLocalizacoes_Tick" Interval="30000">
                                </asp:Timer>
                                <div style="float: left; width: auto;">
                                    <div style="float: left; width: auto;">
                                        <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Minion Pro Cond" Font-Size="Medium" Text="Veiculos:"></asp:Label>
                                        <asp:DropDownList ID="ddlVeiculos" runat="server" AutoPostBack="True" DataSourceID="ODSVeiculos" DataTextField="Veiculo" DataValueField="Vei_Codigo" OnSelectedIndexChanged="ddlVeiculos_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <asp:DropDownList ID="ddlVeiculosCere" runat="server" AutoPostBack="True" DataSourceID="ODSVeiculosCere" DataTextField="Veiculo" DataValueField="Vei_Codigo" OnSelectedIndexChanged="ddlVeiculos_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <br />
                                        <asp:CheckBox ID="chAtualizacaoPagina" runat="server" OnCheckedChanged="chAtualizacaoPagina_CheckedChanged" Text="Atualização automática" AutoPostBack="True" Visible="False" />
                                        <br />
                                    </div>
                                    <div style="float: left; width: auto;">
                                    <asp:ImageButton ID="btnRefreshNo" runat="server" Height="50px" ImageUrl="~/images/Refresh no.png" ToolTip="Desativar refresh automático de página" Width="50px" OnClick="btnRefreshNo_Click" />
                                    <asp:ImageButton ID="btnRefresh" runat="server" Height="50px" ImageUrl="~/images/Refresh.png" ToolTip="Ativar refresh automático de página" Width="50px" OnClick="btnRefresh_Click" />
                                    <asp:ImageButton ID="btnCarregaLocal" runat="server" Height="50px" ImageUrl="~/images/Moniteur Apple Cinema.png" OnClick="btnCarregaLocal_Click" ToolTip="Carregar localizações" Width="50px" Visible="False" />
                                    <asp:ImageButton ID="btnLocalizar" runat="server" Height="50px" ImageUrl="~/images/network2.png" OnClick="btnLocalizar_Click" ToolTip="Solicitar localização do veículo" Visible="False" Width="50px" />
                                    <asp:ImageButton ID="btnLocaliza24Horas" runat="server" Height="50px" ImageUrl="~/images/Alpha Dista Icon 09.png" OnClick="btnLocaliza24Horas_Click" ToolTip="Localizar veículo 24 horas" Width="50px" Visible="False" />
                                    <asp:ImageButton ID="btnParaLocalizacao" runat="server" Height="50px" ImageUrl="~/images/Alpha Dista Icon 09_2.png" OnClick="btnParaLocalizacao_Click" ToolTip="Desabilitar localização 24 horas" Width="50px" Visible="False" />
                                    <asp:ImageButton ID="btnRelData" runat="server" Height="50px" ImageUrl="~/images/NetPrinter.png" OnClick="btnRelData_Click" ToolTip="Relatório por data" Width="50px" />
                                    <asp:ImageButton ID="btnRelViagens" runat="server" Height="50px" ImageUrl="~/images/Lightbrown-Themes-WIP-128x128.png" OnClick="btnRelViagens_Click" ToolTip="Relatório de tráfego" Width="50px" />
                                    <asp:ImageButton ID="btnRelPercurso" runat="server" Height="50px" ImageUrl="~/images/Percurso.png" OnClick="btnRelPercurso_Click" ToolTip="Relatório de percurso" Width="50px" Visible="False" />
                                    <asp:ImageButton ID="btnAvisos" runat="server" Height="50px" ImageUrl="~/images/Info.png" ToolTip="Alertas e informações" Visible="False" Width="50px" />
                                    <asp:ImageButton ID="btnPagamento" runat="server" ImageUrl="~/images/Codigo Barras.png" OnClick="btnPagamento_Click" Width="70px" />
                                    <asp:ImageButton ID="btnContato" runat="server" Height="50px" ImageUrl="~/images/home-phone.png" OnClick="btnContato_Click" ToolTip="Contatos" Width="50px" />
                                    <asp:ImageButton ID="btnFrota" runat="server" Height="50px" ImageUrl="~/images/Camion.png" OnClick="btnFrota_Click" ToolTip="Controle de Frota" Width="50px" />
                                    </div>
                                </div>
                                <div style="float: right; width: auto;">
                                    <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Right">
                                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="docs/Contrato RastroSegue.pdf" Target="_blank" Font-Size="Small" ForeColor="#FF3300">Contrato de adesão</asp:HyperLink>
                                    </asp:Panel>
                                </div>
                                <div style="float: right; width: auto;">
                                    <asp:Panel ID="pnFatura" runat="server">
                                        <h5>Fatura Atual</h5>
                                        <div style="float: left; width: auto;">
                                            <asp:Label ID="Label5" runat="server" Text="Vencimento" ForeColor="#999999" Font-Size="Small"></asp:Label>
                                            <br />
                                            <asp:Label ID="lblVencimento" runat="server" Font-Bold="True" Font-Size="Small"></asp:Label>
                                            <br />
                                            <asp:HyperLink ID="hplLinkFatura" runat="server" NavigateUrl="Doc-Faturas.aspx" Font-Size="Small" ForeColor="#FF3300">Ver detalhes</asp:HyperLink>
                                        </div>
                                        <div style="float: left; width: auto;">
                                            <asp:Label ID="Label6" runat="server" Text="Total" ForeColor="#999999"></asp:Label>
                                            <br />
                                            <asp:Label ID="lblTotalFatura" runat="server" Font-Bold="True" Font-Size="Small"></asp:Label>
                                            &nbsp;&nbsp;&nbsp;
                                        </div>
                                        <div style="float: left; width: auto;">
                                            <asp:Label ID="Label7" runat="server" Text="Status" ForeColor="#999999"></asp:Label>
                                            <br />
                                            <asp:Label ID="lblStatus" runat="server" Font-Bold="True" Font-Size="Small"></asp:Label>
                                        </div>

                                    </asp:Panel>
                                </div>
                                <br />
                                <br />
                                <br />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">                            
                            <ContentTemplate>                        
                                <asp:Panel ID="pnRelData" runat="server" HorizontalAlign="Center">
                                    <asp:Label ID="Label2" runat="server" Text="Relatório por data" Font-Italic="True" Font-Bold="True" Font-Size="Medium" ForeColor="#0066FF"></asp:Label>
                                    <br />
                                    &nbsp;<asp:Label ID="Label3" runat="server" Text="Dt. de pesquisa:"></asp:Label>
                                    &nbsp;<asp:TextBox ID="txtDtFinal" runat="server"></asp:TextBox>

                                    <cc1:MaskedEditExtender ID="txtDtFinal_MaskedEditExtender" runat="server" ClearMaskOnLostFocus="False" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" Mask="99/99/9999" TargetControlID="txtDtFinal">
                                    </cc1:MaskedEditExtender>

                                    &nbsp;<asp:Button ID="btnExibeRelData" runat="server" Text="Exibir" Width="100px" OnClick="btnExibeRelData_Click" />
                                </asp:Panel>
                                <asp:GridView ID="gvLocalizacoes" runat="server" AllowPaging="True" AutoGenerateColumns="False" CellPadding="4" EnableModelValidation="True" ForeColor="#333333" GridLines="None" OnPageIndexChanging="gvLocalizacoes_PageIndexChanging" OnSelectedIndexChanged="gvLocalizacoes_SelectedIndexChanged" PageSize="4" Width="100%">
                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    <Columns>
                                        <asp:CommandField SelectText="Exibir no mapa" ShowSelectButton="True" />
                                        <asp:HyperLinkField DataNavigateUrlFields="His_Latitude,His_Longitude" DataNavigateUrlFormatString="Adm-GoogleMaps.aspx?mLatitude={0}&amp;mLongitude={1}" DataTextField="GoogleMaps" HeaderText="Nível de solo" Target="_blank" />
                                        <asp:BoundField DataField="Loc_Codigo" HeaderText="Código" />
                                        <asp:BoundField DataField="Vei_Placa" HeaderText="Placa" />
                                        <asp:BoundField DataField="His_Data" HeaderText="Data" />
                                        <asp:BoundField DataField="His_Hora" HeaderText="Hora" />
                                        <asp:BoundField DataField="His_Latitude" HeaderText="Latitude" />
                                        <asp:BoundField DataField="His_Longitude" HeaderText="Longitude" />
                                        <asp:BoundField DataField="His_Speed" HeaderText="Velocidade" />
                                        <asp:BoundField DataField="His_Status" HeaderText="Status" />
                                    </Columns>
                                    <EditRowStyle BackColor="#999999" />
                                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Font-Size="Small" />
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                </asp:GridView>                                                      
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <asp:Panel ID="Panel2" runat="server">
                        <div id='mapdiv' style="position: relative; width: 100%; height: 500px;"></div>
                    </asp:Panel>


                    <asp:TextBox ID="txtVeiculo" runat="server" Text="" Width="1px" BackColor="#212222"></asp:TextBox>
                    <asp:TextBox ID="txtLatitude" runat="server" BackColor="#212222" Width="1px"></asp:TextBox>
                    <asp:TextBox ID="txtLongitude" runat="server" BackColor="#212222" Width="1px"></asp:TextBox>
                    <asp:TextBox ID="txtZoom" runat="server" BackColor="#212222" Width="1px"></asp:TextBox>
                    <asp:TextBox ID="txtStatus" runat="server" BackColor="#212222" Width="1px"></asp:TextBox>
                    <asp:TextBox ID="txtIconeColor" runat="server" BackColor="#212222" Width="1px"></asp:TextBox>
                    <asp:TextBox ID="txtTextoPin" runat="server" BackColor="#212222" Width="1px"></asp:TextBox>
                    <asp:TextBox ID="txtArquivo" runat="server" BackColor="#212222" Width="1px"></asp:TextBox>
                    <!--/row-->
                </div>
            </div>
        </div>

        <div class="clearfix"></div>

        <footer>
            <p>
                <span style="text-align: left; float: left">&copy; 2016 <a href="#" alt="FP Soluções Tecnológicas LTDA">FP Soluções Tecnológicas LTDA</a></span>
            </p>
        </footer>

        <!-- start: JavaScript-->

        <script src="js2/jquery-1.9.1.min.js"></script>
        <script src="js2/bootstrap.min.js"></script>
        <script src="js2/custom.js"></script>
               
    </form>

</body>
</html>

