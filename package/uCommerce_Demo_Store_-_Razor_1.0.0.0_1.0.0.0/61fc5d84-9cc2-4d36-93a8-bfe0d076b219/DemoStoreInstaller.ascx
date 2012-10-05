<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DemoStoreInstaller.ascx.cs" Inherits="uCommerce.RazorStore.Umbraco.UCommerce.Install.DemoStoreInstaller" %>
<p>Thanks for installing the demo uCommerce store. We've not installed any products yet so you have a completely blank install.</p>
<p><asp:Button runat="server" ID="btnInstall" Text="Install Example Products" OnClick="btnInstall_Click"/></p>
<div class="well sidebar-nav" id="newsletter">
    <h5>Keep up-to-date</h5>
    <p>Sign up to our newsletter.</p>
    <div id="newsletter-form">
        <label for="name">Name:</label><br /><input type="text" name="cm-name" id="name" /><br />
        <label for="xqlur-xqlur">Email:</label><br /><input type="text" name="cm-xqlur-xqlur" id="xqlur-xqlur" class="required email" /><br />
        <input type="checkbox" name="cm-ol-xbjtk" id="AvenueClothingSignup" /><label for="AvenueClothingSignup">Also sign up for uCommerce retailer news</label><br />
        <button type="submit" class="btn">Go</button>
	</div>
</div>
<script src="//ajax.googleapis.com/ajax/libs/jquery/1.8.1/jquery.min.js"></script>
<script>
    $(function () {
        $('#newsletter-form button').click(function (e) {
            e.preventDefault();
            var form = $('#newsletter-form');
            $.getJSON("http://thesitedoctor.createsend.com/t/y/s/xqlur/?callback=?", $('input', form).serialize(), function (data) {
                console.log(data);
                var thanks = $('<p />', { text: "Thanks! We\'ll be in touch soon!" });
                if (data.Status === 400) {
                    alert("Error: " + data.Message);
                    form.css('opacity', 1);
                    $('input:first', form).focus();
                } else { // 200
                    form.fadeOut(300, function () { $(this).css('opacity', 1).html(thanks).fadeIn(300); });
                }
            });
            return false;
        });
    });
</script>