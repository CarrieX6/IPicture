<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Publish.aspx.cs" Inherits="Publish" %>

<%@ Register src="HeadControl.ascx" tagname="HeadControl" tagprefix="uc1" %>

<%@ Register src="FootControl.ascx" tagname="FootControl" tagprefix="uc2" %>

<!DOCTYPE HTML>
<html class="no-js" lang="en">
	<head>
		<meta charset="utf-8">
		<meta http-equiv="x-ua-compatible" content="ie=edge">
		<meta name="author" content="smartit-source">
		<meta name="description" content="">
		<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">	
		<!-- title here -->
		<title>IPicture</title>		
        <!-- Favicon and Touch Icons -->
        <link rel="shortcut icon" href="assets/images/fav.png">
		<!-- Place favicon.ico in the root directory -->
		<link rel="apple-touch-icon" href="apple-touch-icon.png">
		<!-- Fonts -->
		<link href="https://fonts.googleapis.com/css?family=Poppins:300,400,500,600,700,800,900" rel="stylesheet">
		<link rel="stylesheet" href="assets/css/font-awesome.min.css">
		<link rel="stylesheet" href="assets/css/flaticon.css">
		<!-- Plugin CSS -->
		<link rel="stylesheet" href="assets/css/bootstrap.min.css">
		<link rel="stylesheet" href="assets/css/animate.css">
		<link rel="stylesheet" href="assets/css/magnific-popup.css">
		<link rel="stylesheet" href="assets/css/owl.carousel.css">
		<link rel="stylesheet" href="assets/css/owl.theme.css">
		<link rel="stylesheet" href="assets/css/owl.transitions.css">	
		<link rel="stylesheet" href="assets/css/jquery.barfiller.css">							
		<!--Theme custom css -->
		<link rel="stylesheet" href="assets/css/style.css">
		<!--Theme Responsive css-->
		<link rel="stylesheet" href="assets/css/responsive.css" />
		<script src="assets/js/vendor/modernizr-2.8.3-respond-1.4.2.min.js"></script>
        <script type="text/javascript">
            function DataCheck()
            {
                var title = $("#txtTitle").val();
                var describe = $("#txtdescribe").val();
                var pic = $("#HidPic").val();

                if (title == undefined || title == "")
                {
                    alert("Title must be filled!");
                    return false;
                }

                if (describe == undefined || describe == "") {
                    alert("Describe must be filled!");
                    return false;
                }

                if (pic == undefined || pic == "") {
                    alert("Picture must be Uploaded!");
                    return false;
                }
                return true;
            }
        </script>
	</head>
	<body>
		<form id="form1" runat="server">
		<!-- header area start here -->
        <uc1:HeadControl ID="HeadControl1" runat="server" />
		<!-- header area start here -->
		<!-- breadcrumb area start here	 -->
		    
		<div class="breadcrumb-area" style="padding-top:100px">
		</div>
		<!-- breadcrumb area end here	 -->	
		<!-- portfolio area start here	 -->
		<div class="blog-details-area section">
			<div class="container">
				<div class="row">
					<div class="col-lg-8">
						<div class="comment-area box-shadow mt-10">
							<div class="comment-from mt-63">
								<div class="section-title">
									<h3>Picture Publish</h3>
								</div>
									<div class="row">
                                        
										<div class="col-lg-12">
											<div class="form-group">
												<input type="text" class="form-control" style="width:100%" id="txtTitle" name="txtTitle" placeholder="Title" runat="server">
											</div>
										</div>
										
                                        <div class="col-lg-6">
											<div class="form-group">
                                                <asp:FileUpload ID="FileUp" runat="server" Width="100%"/>
                                                
											</div>
										</div>

                                        <div class="col-lg-6">
											<div class="form-group">
                                                <asp:Button ID="btnUp" runat="server" Text="Upload Photo" OnClick="btnUp_Click" />
                                                <asp:HiddenField ID="HidPic" runat="server" />
											</div>
										</div>

                                        <div class="col-lg-12">
											<div class="form-group">
                                                <asp:CheckBoxList ID="chkCartegory" Width="100%" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList>
											</div>
										</div>

										<div class="col-lg-12">
											<div class="form-group">
												<textarea name="describe" id="txtdescribe" cols="30" rows="10" style="width:100%" placeholder="Message" runat="server"></textarea>
											</div>
										</div>

                                        <div class="col-lg-12">
											<div class="from-btn">
                                                <asp:Button ID="btnPublish" runat="server" CssClass="primary-btn" Text="Publish" OnClientClick="return DataCheck()" OnClick="btnPublish_Click"/>
											</div>
										</div>
									</div>
							</div>
						</div>				
					</div>
				</div>
			</div>
			<!-- leaf left area start here	 -->
			<div class="leaf-left">
				<img src="assets/images/leaf-left.png" alt="leaf-right">
			</div>
			<!-- leaf left area end here	 -->
			<!-- leaf right area start here	 -->
			<div class="leaf-right">
				<img src="assets/images/leaf-right.png" alt="leaf-right">
			</div>
			<!-- leaf right area end here	 -->
		</div>	
		<!-- portfolio area start here	 -->		
		<!-- footer area start here -->
        <uc2:FootControl ID="FootControl1" runat="server" />
		<!-- footer area end here -->		
		<!-- js file start -->
		<script src="assets/js/vendor/jquery-1.12.0.min.js"></script>
		<script src="assets/js/plugins.js"></script>
		<script src="assets/js/Popper.js"></script>
		<script src="assets/js/bootstrap.min.js"></script>
		<script src="assets/js/jquery.magnific-popup.min.js"></script>
		<script src="assets/js/owl.carousel.min.js"></script>
		<script src="assets/js/isotope.pkgd.min.js"></script>
		<script src="assets/js/imagesloaded.pkgd.min.js"></script>	
		<script src="assets/js/scrollup.js"></script>	
		<script src="assets/js/jquery.counterup.min.js"></script>	
		<script src="assets/js/waypoints.min.js"></script>	
		<script src="assets/js/jquery.appear.js"></script>	
		<script src="assets/js/jquery.barfiller.js"></script>																																		
		<script src="assets/js/main.js"></script>
		<!-- End js file -->
	    </form>
	</body>
</html>