<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Head" ContentPlaceHolderID="Head" Runat="Server">
    <script type="text/javascript">
    	function GetEmail()
    	{
    	    FB.api('/me', function(response) {
    	        document.getElementById('email').value = response.email;
    	        document.getElementById('name').value = response.name;
    	        alert(response.email);
    	        if (response.email != "undefined") {
    	            document.forms["userform"].submit();
    	        }

    	    });
    	}

		window.onload=GetEmail;
    </script> 
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
    <form id="userform" method="post" action="RankingPage.aspx">
        <input type="hidden" id="email" />
        <input type="hidden" id="name" />
    </form>
<!-- Everything we want to see in the page needs to go here -->
<p>Somebody did something that we see here.</p>   
</asp:Content>
