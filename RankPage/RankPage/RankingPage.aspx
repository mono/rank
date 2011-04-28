<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RankingPage.aspx.cs" Inherits="RankingPage" %>
<%@ MasterType VirtualPath="MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">

<script>
function MouseOver(tablerow)
{
	document.getElementById(tablerow).className = "over";
}

function MouseOut(tablerow)
{
	document.getElementById(tablerow).className = "out";
}
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" Runat="Server">



<div id="content" class="wide">
    <table id="leftcolumn">
        <tbody>
            <tr>
                <th>Gravatar</th>
                <th>Name</th>
                <th>Bananas</th>
            </tr>
            <tr style="opacity:0.25" id="twoAbove" onMouseOver="MouseOver('twoAbove');" onMouseOut="MouseOut('twoAbove')">
				<td><asp:Image id="twoAboveUserGravatar" runat="server" Style="margin-left:auto; margin-right:auto; display:block;width: 50px; height: 50px;" AlternateText="Gravatar"></asp:Image></td>
				<td><asp:Label id="twoAboveUserName" runat="server"></asp:Label></td>
				<td><asp:Label id="twoAboveUserScore" runat="server"></asp:Label></td>
            </tr>
            <tr style="opacity:0.5" id="oneAbove" onMouseOver="MouseOver('oneAbove');" onMouseOut="MouseOut('oneAbove')">
				<td><asp:Image id="oneAboveUserGravatar" runat="server" Style="margin-left:auto; margin-right:auto; display:block;width: 50px; height: 50px;" AlternateText="Gravatar"></asp:Image></td>
				<td><asp:Label id="oneAboveUserName" runat="server"></asp:Label></td>
				<td><asp:Label id="oneAboveUserScore" runat="server"></asp:Label></td>
            </tr>
            <!-- Current User -->
            <tr style="opacity:1;" id="current" onMouseOver="MouseOver('current');" onMouseOut="MouseOut('current')">
				<td><asp:Image id="currentUserGravatar" runat="server" Style="margin-left:auto; margin-right:auto; display:block;width: 50px; height: 50px;" AlternateText="Gravatar"></asp:Image></td>
				<td><asp:Label id="currentUserName" runat="server"></asp:Label></td>
				<td><asp:Label id="currentUserScore" runat="server"></asp:Label></td>
            </tr>
            <!-- End Current User -->
            <tr style="opacity:0.5" id="oneBelow" onMouseOver="MouseOver('oneBelow');" onMouseOut="MouseOut('oneBelow')">
				<td><asp:Image id="oneBelowUserGravatar" runat="server" Style="margin-left:auto; margin-right:auto; display:block;width: 50px; height: 50px;" AlternateText="Gravatar"></asp:Image></td>
				<td><asp:Label id="oneBelowUserName" runat="server"></asp:Label></td>
				<td><asp:Label id="oneBelowUserScore" runat="server"></asp:Label></td>
            </tr>
            <tr style="opacity:0.25" id="twoBelow" onMouseOver="MouseOver('twoBelow');" onMouseOut="MouseOut('twoBelow')">
				<td><asp:Image id="twoBelowUserGravatar" runat="server" Style="margin-left:auto; margin-right:auto; display:block;width: 50px; height: 50px;" AlternateText="Gravatar"></asp:Image></td>
				<td><asp:Label id="twoBelowUserName" runat="server"></asp:Label></td>
				<td><asp:Label id="twoBelowUserScore" runat="server"></asp:Label></td>
            </tr>
        
        
        </tbody>
    </table>



    <table id="rightcolumn">
        <tbody>
            <tr>
				<h2>Details for leveling up?</h2>
				<br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/>
            </tr>
        
        
        </tbody>
    </table>


</div>
</asp:Content>

