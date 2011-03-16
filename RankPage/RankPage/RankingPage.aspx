<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RankingPage.aspx.cs" Inherits="RankingPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" Runat="Server">
<div id="leftcolumn">
    <table>
        <tbody>
            <tr>
                <th>Gravatar</th>
                <th>Name</th>
                <th>Bananas</th>
            </tr>
            <tr style="opacity:0.25">
                <td><img src="http://www.gravatar.com/avatar/3a9106e0c085d9a856588c454894d66b?d=identicon&s=50&r=g" id="avatarimg1" style="margin-left:auto; margin-right:auto; display:block;width: 50px; height: 50px;" alt="Gravatar" /></td>
                <td>Carsten Schlote</td>
                <td>511071</td>
            </tr>
            <tr style="opacity:0.5">
                <td><img src="http://www.gravatar.com/avatar/146ab0e304cf866e1a1dd9f4f9a8128d?d=identicon&s=50&r=g" id="avatarimg2"  style="margin-left:auto; margin-right:auto; display:block;width: 50px; height: 50px;" alt="Gravatar" /></td>
                <td>Mike Krüger</td>
                <td>490852</td>
            </tr>
            <!-- Current User -->
            <tr>
				<td><asp:Image id="currentUserGravatar" runat="server" Style="margin-left:auto; margin-right:auto; display:block;width: 50px; height: 50px;" AlternateText="Gravatar"></asp:Image></td>
				<td><asp:Label id="currentUserName" runat="server"></asp:Label></td>
				<td><asp:Label id="currentUserScore" runat="server"></asp:Label></td>
            </tr>
            <!-- End Current User -->
            <tr style="opacity:0.5">
                <td><img src="http://www.gravatar.com/avatar/9bbe9342b19d5815d5b8e78154287c06?d=identicon&s=50&r=g" id="avatarimg4"  style="margin-left:auto; margin-right:auto; display:block;width: 50px; height: 50px;" alt="Gravatar" /></td>
                <td>Marek Safar</td>
                <td>145479</td>
            </tr>
            <tr style="opacity:0.25">
                <td><img src="http://www.gravatar.com/avatar/c72e3d0e20153a42eed12b75f8a91016?d=identicon&s=50&r=g" id="avatarimg5"  style="margin-left:auto; margin-right:auto; display:block;width: 50px; height: 50px;" alt="Gravatar" /></td>
                <td>Jeffrey Stedfast</td>
                <td>82572</td>
            </tr>
            
        
        
        </tbody>
    </table>

</div>
<div id="rightcolumn">
<h2>Details for leveling up?</h2>
<p>Some information is going to go in here</p>
    <asp:Button ID="btnShowHow" runat="server" Text="Show me how" 
        onclick="btnShowHow_Click" />

</div>
</asp:Content>

