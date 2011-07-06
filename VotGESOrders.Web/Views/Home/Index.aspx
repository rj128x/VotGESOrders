<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Домашняя страница
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: ViewData["Message"] %></h2>
    <p>
        Для получения дополнительных сведений о ASP.NET MVC посетите веб-сайт <a href="http://asp.net/mvc" title="Веб-сайт ASP.NET MVC">http://asp.net/mvc</a>.
    </p>
</asp:Content>
