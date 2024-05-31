var csrfToken = 'CSRF_TOKEN_COLLAB_FORM5';

//my scripts
function intRegPG() {
    $(document).ready(function () {
        $('#loadTerms').on('click',
            function (e) {
                displayModal(
                    '/Home/TermsAndConditionsPartial', 'Terms And Conditions', this.id, 'small');
            });
    });

    $(document).ready(function () {
        $('#loadPrivacyPolicy').on('click',
            function (e) {
                displayModal(
                    '/Home/PrivacyPolicyPartial', 'Privacy Policy', this.id, 'lg');
            });
    });
};

//ajax load client
var pageSize = 6;
var pageIndex = 0;
var _incallback = false;
var _EOTL = false;
//ajax load client

//ajax load clients
function GetClientData(isnew) {

    if (isnew) {
        pageIndex = 0; _EOTL = false;
    }

    $.ajax({
        type: 'GET',
        url: '/User/LoadClients',
        data: { "pageindex": pageIndex, "pagesize": pageSize },
        dataType: 'json',
        success: function (response) {
            if (response != null && response.length > 0) {
                if (isnew) {
                    $("#searchResults").html("");
                }
                if (response[0].server_Res && response[0].server_Res === "EOTL") {
                    $("#searchResults").append("<div class='col-12 text-center'><h5>Niente pi&ugrave; record</h5></div>");
                    _EOTL = true;
                } else {

                    for (var i = 0; i < response.length; i++) {
                        var _rentOrSale;
                        if (response[i].sale) {
                            _rentOrSale = "Vendita";
                        } else {
                            _rentOrSale = "Affitto";
                        }

                        var _progressClass;
                        if (response[i].progressItemId == 1) {
                            _progressClass = "info";
                        }
                        if (response[i].progressItemId == 2) {
                            _progressClass = "success";
                        }
                        if (response[i].progressItemId == 3) {
                            _progressClass = "warning";
                        }
                        if (response[i].progressItemId == 4) {
                            _progressClass = "danger";
                        }

                        $("#searchResults").append("<div class='card mb-4'><div class='card-body text-center'><div class='card-content'><h5>" + response[i].name + " " + response[i].surName + "</h5><p class='mb-1'>Ph# " + response[i].phone + "</p><p class='mb-1'>" + _rentOrSale + "</p><p class='mb-1'>Indirizzo: " + response[i].address + "</p><p class='mb-1 text-" + _progressClass + "'>Progresso: " + response[i].progressName + "</p><a class='mb-1 text-" + _progressClass + "'>Provvigione stimate: <span class='badge bg-" + _progressClass + " rounded-pill'> &#128; " + response[i].estimatedCommitssion + "</span></a><br><a href='/User/ClientStatus/" + response[i].id + "' class='btn btn-primary w-100 mt-2'><span>Stato Segnalazioni</span></a></div></div></div>");

                    }
                    pageIndex++;
                }
            }
        },
        beforeSend: function () {
            $("#progress").show();
            $("#progress").append("<i class='ml-1 bi bi-arrow-repeat icon-spin'></i>");
        },
        complete: function (event, jqXHR) {
            if (jqXHR.toLocaleLowerCase() === "success") {
                $('#progress').find('i').remove();
                $("#progress").hide();
            }
            else {
                $('#progress').find('i').remove();
                $("#progress").append("<i class='text-danger ml-1 bi bi-exclamation-octagon'></i>");
            }
            _incallback = false;
        },
        error: function (jqXHR, exception) {
            var msg = '';
            if (jqXHR.status === 0) {
                msg = 'Not connected. Verify Network.';
            } else if (jqXHR.status == 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status == 500) {
                msg = 'Internal Server Error [500].';
            } else if (exception === 'parsererror') {
                msg = 'Requested JSON parse failed.';
            } else if (exception === 'timeout') {
                msg = 'Time out error.';
            } else if (exception === 'abort') {
                msg = 'Ajax request aborted.';
            } else {
                msg = 'Uncaught Error.\n' + jqXHR.responseText;
                console.error(jqXHR);
            }

            $("#progress").append("<i class='text-danger ml-1 bi bi-exclamation-octagon'></i>");
            console.log(msg);
            _incallback = false;
        }
    });
}
//ajax load clients

//ajax load users
function GetUsersData(isnew) {

    if (isnew) {
        pageIndex = 0; _EOTL = false;
    }

    $.ajax({
        type: 'GET',
        url: '/User/LoadUsers',
        data: { "pageindex": pageIndex, "pagesize": pageSize },
        dataType: 'json',
        success: function (response) {
            if (response != null && response.length > 0) {
                if (isnew) {
                    $("#searchResults").html("");
                }
                if (response[0].server_Res && response[0].server_Res === "EOTL") {
                    $("#searchResults").append("<div class='col-12 text-center'><h5>Niente pi&ugrave; record</h5></div>");
                    _EOTL = true;
                } else {

                    for (var i = 0; i < response.length; i++) {

                        var _emailConfirmed;
                        if (response[i].emailConfirmed) {
                            _emailConfirmed = "Email Confirmed";
                        } else {
                            _emailConfirmed = "Email Not Confirmed";
                        }

                        var _roleClass;
                        if (response[i].role == "Admin") {
                            _roleClass = "danger";
                        }
                        if (response[i].role == "User") {
                            _roleClass = "success";
                        }

                        $("#searchResults").append("<div id='cardUsr_" + response[i].id + "' class='card mb-4'><div class='card-body text-center'><div class='card-content'><h5>" + response[i].name + "</h5><p class='mb-1'>Email: " + response[i].email + "</p><p class='mb-1'>" + _emailConfirmed + "</p><p class='mb-1'>Invited By: " + response[i].invitedBy + "</p><p class='mb-1 text-" + _roleClass + "'>Role: " + response[i].role + "</p><p class='mb-1'>Clients Added by User: " + response[i].clientsAdded + "</p><br><a id='btnDelUsr_" + response[i].id + "' onclick=delUsrFn('" + response[i].id + "',this.id,'cardUsr_" + response[i].id + "') class='btn btn-danger w-100 mt-2'><span>Delete User/added clients by this user</span></a></div></div></div>");

                    }
                    pageIndex++;
                }
            }
        },
        beforeSend: function () {
            $("#progress").show();
            $("#progress").append("<i class='ml-1 bi bi-arrow-repeat icon-spin'></i>");
        },
        complete: function (event, jqXHR) {
            if (jqXHR.toLocaleLowerCase() === "success") {
                $('#progress').find('i').remove();
                $("#progress").hide();
            }
            else {
                $('#progress').find('i').remove();
                $("#progress").append("<i class='text-danger ml-1 bi bi-exclamation-octagon'></i>");
            }
            _incallback = false;
        },
        error: function (jqXHR, exception) {
            var msg = '';
            if (jqXHR.status === 0) {
                msg = 'Not connected. Verify Network.';
            } else if (jqXHR.status == 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status == 500) {
                msg = 'Internal Server Error [500].';
            } else if (exception === 'parsererror') {
                msg = 'Requested JSON parse failed.';
            } else if (exception === 'timeout') {
                msg = 'Time out error.';
            } else if (exception === 'abort') {
                msg = 'Ajax request aborted.';
            } else {
                msg = 'Uncaught Error.\n' + jqXHR.responseText;
                console.error(jqXHR);
            }

            $("#progress").append("<i class='text-danger ml-1 bi bi-exclamation-octagon'></i>");
            console.log(msg);
            _incallback = false;
        }
    });
}
//ajax load users

function delUsrFn(id, thisElement,cardElement) {
    event.preventDefault();
    usrDelTmpId = id; usrDelTmpElement = thisElement; usrDelTmpCardElement = cardElement;

    swal({
        title: "Are you sure?",
        text: "Once deleted, this User and all of its added clients data will be lost, deleted user will not be able login but can register again.",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {

            let tag = {
                "UsrId": id
            };

            $("#" + thisElement + "").append("<i class='ml-1 bi bi-arrow-repeat icon-spin'></i>").prop('disabled', true);

            $.ajax(
                {
                    url: '/User/DeleteUser',
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(tag),
                    dataType: 'json',
                    headers: {
                        RequestVerificationToken:
                            $('input:hidden[name=' + csrfToken + ']').val()
                    },
                    async: true,
                    success: function (data) {
                        dUsrAjaxSuccess(data, usrDelTmpId, usrDelTmpElement, usrDelTmpCardElement);
                    },
                    error: function (req, status) {
                        dUsrAjaxError(req, status, usrDelTmpElement);
                    }
                })


        } else {
            //swal("Record was not deleted!");
        }
    });

};

function dUsrAjaxSuccess(data, id, thisElement,cardElement) {
    if (data.status == 'error') {

        $("#" + thisElement + "").find('i').remove();
        $("#" + thisElement + "").append("<i class='text-danger ml-1 bi bi-exclamation-octagon'></i>").prop('disabled', false);
        swal("" + data.errors + "", {
            icon: "error",
            timer: 8000,
        });
        console.error(data.errors);

    } else if (data.status == 'success') {

        $("#" + cardElement + "").remove();

        swal("" + data.message + "", {
            icon: "success",
            timer: 5000,
        });

    }

};

function dUsrAjaxError(req, status, thisElement) {
    $("#" + thisElement + "").find('i').remove();
    $("#" + thisElement + "").append("<i class='text-danger ml-1 bi bi-exclamation-octagon'></i>").prop('disabled', false);
    swal("Request Failed!", {
        icon: "error",
        timer: 5000,
    });
    console.error(status);
};

function displayModal(url, heading, componetId, modalSize, modelName = "dynamicModal") {
    if (componetId) {
        $("#" + componetId + "").append("<i class='bi bi-arrow-repeat icon-spin ms-1'></i>");
    }

    $.get(url, function (data) {
        doModal(heading, data, modalSize, modelName);
        if (componetId) {
            $("#" + componetId + "").find('i', '.bi.bi-arrow-repeat.icon-spin.ms-1').remove();
        };
    }).fail(function () {
        if (componetId) {
            $("#" + componetId + "").find('i', '.bi.bi-arrow-repeat.icon-spin.ms-1').remove();
            $("#" + componetId + "").append("<i class='text-danger bi bi-exclamation-octagon ms-1'></i>");
        };
    });

};

function doModal(heading, formContent, size, modelName) {
    html = '<div id="' + modelName + '" class="modal fade" tabindex="-1" data-bs-keyboard="false" aria-labelledby="' + modelName + 'Title" aria-hidden="true" data-bs-backdrop="static">';
    html += '<div class="modal-dialog modal-dialog-centered modal-' + size + '">';
    html += '<div class="modal-content">';

    html += '<div class="modal-header">';
    html += '<h4 class="modal-title" id="' + modelName + 'Title">' + heading + '</h4>';
    html += '<button type="button" class="btn btn-close p-1 ms-auto" data-bs-dismiss="modal" aria-hidden="true"></button>';
    html += '</div>';

    html += '<div class="modal-body">';
    html += formContent;
    html += '</div>';

    html += '<div class="modal-footer">';
    html += '<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>';
    html += '</div>';

    html += '</div>';//modalContent
    html += '</div>';
    html += '</div>';  // modalWindow

    $('body').append(html);
    $("#" + modelName + "").modal();
    $("#" + modelName + "").modal('show');

    $('#' + modelName + '').on('hidden.bs.modal', function (e) {
        $(this).remove();
    });

};

async function checkNet() {
    try {
        const _response = await fetch('.');
        if (_response.status >= 200 && _response.status < 500) {
            //history.back();
            window.location.reload();
            return;
        }
    } catch (e) {

    }
    window.setTimeout(checkNet, 2500);
}

//my scripts end
(function ($) {
    'use strict';

    var affanWindow = $(window);

    // :: 1.0 Preloader

    affanWindow.on('load', function () {
        $('#preloader').fadeOut('1000', function () {
            $(this).remove();
        });
    });

    // :: 3.0 Dropdown Menu

    $(".sidenav-nav").find("li.affan-dropdown-menu").append("<div class='dropdown-trigger-btn'><i class='fa fa-angle-right'></i></div>");
    $(".dropdown-trigger-btn").on('click', function () {
        $(this).siblings('ul').stop(true, true).slideToggle(400);
        $(this).toggleClass('active');
    });

    //// :: 4.0 Setting Trigger

    //$("#settingTriggerBtn").on("click", function () {
    //    $("#settingCard").toggleClass("active");
    //    $("#setting-popup-overlay").toggleClass("active");
    //});

    //// :: 5.0 Setting Card

    //$("#settingCardClose").on("click", function () {
    //    $("#settingCard").removeClass("active");
    //    $("#setting-popup-overlay").removeClass("active");
    //});    

    //// :: 7.0 Video Calling Button

    //$("#videoCallingButton").on("click", function () {
    //    $("#videoCallingPopup").addClass('screen-active');
    //    $(".chat-wrapper").addClass('calling-screen-active');
    //});

    //$("#videoCallDecline").on("click", function () {
    //    $("#videoCallingPopup").removeClass('screen-active');
    //    $(".chat-wrapper").removeClass('calling-screen-active');
    //});

    //// :: 8.0 Calling Button

    //$("#callingButton").on("click", function () {
    //    $("#callingPopup").addClass('screen-active');
    //    $(".chat-wrapper").addClass('calling-screen-active');
    //});

    //$("#callDecline").on("click", function () {
    //    $("#callingPopup").removeClass('screen-active');
    //    $(".chat-wrapper").removeClass('calling-screen-active');
    //});

    //// :: 9.0 Owl Carousel One

    //if ($.fn.owlCarousel) {
    //    var owlCarouselOne = $('.owl-carousel-one');
    //    owlCarouselOne.owlCarousel({
    //        items: 1,
    //        loop: true,
    //        autoplay: true,
    //        dots: true,
    //        center: true,
    //        margin: 0,
    //        nav: true,
    //        navText: [('<i class="fa fa-angle-left"></i>'), ('<i class="fa fa-angle-right"></i>')]
    //    })

    //    owlCarouselOne.on('translate.owl.carousel', function () {
    //        var layer = $("[data-animation]");
    //        layer.each(function () {
    //            var anim_name = $(this).data('animation');
    //            $(this).removeClass('animated ' + anim_name).css('opacity', '0');
    //        });
    //    });

    //    $("[data-delay]").each(function () {
    //        var anim_del = $(this).data('delay');
    //        $(this).css('animation-delay', anim_del);
    //    });

    //    $("[data-duration]").each(function () {
    //        var anim_dur = $(this).data('duration');
    //        $(this).css('animation-duration', anim_dur);
    //    });

    //    owlCarouselOne.on('translated.owl.carousel', function () {
    //        var layer = owlCarouselOne.find('.owl-item.active').find("[data-animation]");
    //        layer.each(function () {
    //            var anim_name = $(this).data('animation');
    //            $(this).addClass('animated ' + anim_name).css('opacity', '1');
    //        });
    //    });
    //}

    //// :: 10.0 Owl Carousel Two

    //if ($.fn.owlCarousel) {
    //    var owlCarouselTwo = $('.owl-carousel-two');
    //    owlCarouselTwo.owlCarousel({
    //        items: 1,
    //        loop: true,
    //        autoplay: true,
    //        dots: true,
    //        center: true,
    //        margin: 30,
    //        nav: false,
    //        animateIn: 'fadeIn',
    //        animateOut: 'fadeOut'
    //    });

    //    var dot = $('.owl-carousel-two .owl-dot');
    //    dot.each(function () {
    //        var index = $(this).index() + 1;

    //        if (index < 10) {
    //            $(this).html('0' + index);
    //        } else {
    //            $(this).html(index);
    //        }
    //    });

    //    var owlDotsCount = $(".owl-carousel-two .owl-dots").children().length;
    //    if (owlDotsCount < 10) {
    //        $("#totalowlDotsCount").html('0' + owlDotsCount);
    //    } else {
    //        $("#totalowlDotsCount").html(owlDotsCount);
    //    }
    //}

    //// :: 11.0 Owl Carousel Three

    //if ($.fn.owlCarousel) {
    //    var owlCarouselThree = $('.owl-carousel-three');
    //    owlCarouselThree.owlCarousel({
    //        items: 2,
    //        loop: true,
    //        autoplay: true,
    //        dots: false,
    //        center: true,
    //        margin: 8,
    //        nav: false
    //    })
    //}

    //// :: 12.0 Testimonial Slides One

    //if ($.fn.owlCarousel) {
    //    var testimonial1 = $('.testimonial-slide');
    //    testimonial1.owlCarousel({
    //        items: 1,
    //        loop: true,
    //        autoplay: true,
    //        dots: true,
    //        margin: 30,
    //        nav: false
    //    })
    //}

    //// :: 13.0 Testimonial Slides Two

    //if ($.fn.owlCarousel) {
    //    var testimonial2 = $('.testimonial-slide2');
    //    testimonial2.owlCarousel({
    //        items: 2,
    //        loop: true,
    //        autoplay: true,
    //        dots: true,
    //        margin: 0,
    //        nav: true,
    //        navText: [('<i class="fa fa-angle-left"></i>'), ('<i class="fa fa-angle-right"></i>')],
    //        center: true
    //    })
    //}

    //// :: 14.0 Partner Slides

    //if ($.fn.owlCarousel) {
    //    var partnerSlide = $('.partner-slide');
    //    partnerSlide.owlCarousel({
    //        items: 3,
    //        margin: 12,
    //        loop: true,
    //        autoplay: true,
    //        autoplayTimeout: 5000,
    //        dots: true,
    //        nav: false
    //    })
    //}

    //// :: 15.0 Gallery Slides

    //if ($.fn.owlCarousel) {
    //    var galleryCarousel = $('.image-gallery-carousel');
    //    galleryCarousel.owlCarousel({
    //        items: 3,
    //        margin: 8,
    //        loop: true,
    //        autoplay: true,
    //        autoplayTimeout: 5000,
    //        dots: true,
    //        nav: false
    //    })
    //}

    //// :: 16.0 Product Gallery Slides

    //if ($.fn.owlCarousel) {
    //    var productGalleryCarousel = $('.product-gallery');
    //    productGalleryCarousel.owlCarousel({
    //        items: 1,
    //        margin: 0,
    //        loop: true,
    //        autoplay: true,
    //        autoplayTimeout: 5000,
    //        dots: true,
    //        nav: false
    //    })
    //}

    //// :: 17.0 Chat User Slide

    //if ($.fn.owlCarousel) {
    //    var userStatusSlide = $('.chat-user-status-slides');
    //    userStatusSlide.owlCarousel({
    //        items: 5,
    //        margin: 8,
    //        loop: true,
    //        autoplay: false,
    //        autoplayTimeout: 5000,
    //        dots: false,
    //        nav: false,
    //        responsive: {
    //            1200: {
    //                items: 11
    //            },
    //            992: {
    //                items: 10
    //            },
    //            768: {
    //                items: 8
    //            },
    //            576: {
    //                items: 6
    //            },
    //            480: {
    //                items: 5
    //            }
    //        }
    //    })
    //}

    //// :: 18.0 Magnific Popup One

    //if ($.fn.magnificPopup) {
    //    $('#videobtn').magnificPopup({
    //        type: 'iframe'
    //    });
    //    $('.gallery-img').magnificPopup({
    //        type: 'image',
    //        gallery: {
    //            enabled: true
    //        },
    //        removalDelay: 300,
    //        mainClass: 'mfp-fade',
    //        preloader: true,
    //        callbacks: {
    //            beforeOpen: function () {
    //                this.st.image.markup = this.st.image.markup.replace('mfp-figure', 'mfp-figure mfp-with-anim');
    //                this.st.mainClass = this.st.el.attr('data-effect');
    //            }
    //        },
    //        closeOnContentClick: true,
    //        midClick: true
    //    });
    //}

    //// :: 19.0 Magnific Popup Two

    //if ($.fn.magnificPopup) {
    //    $('.gallery-img2').magnificPopup({
    //        type: 'image',
    //        gallery: {
    //            enabled: true
    //        },
    //        removalDelay: 300,
    //        mainClass: 'mfp-fade',
    //        preloader: true,
    //        callbacks: {
    //            beforeOpen: function () {
    //                this.st.image.markup = this.st.image.markup.replace('mfp-figure', 'mfp-figure mfp-with-anim');
    //                this.st.mainClass = this.st.el.attr('data-effect');
    //            }
    //        },
    //        closeOnContentClick: true,
    //        midClick: true
    //    });
    //}

    //// :: 20.0 Masonary Gallery

    //if ($.fn.imagesLoaded) {
    //    $('.gallery-wrapper').imagesLoaded(function () {
    //        // filter items on button click
    //        $('.gallery-menu').on('click', 'button', function () {
    //            var filterValue = $(this).attr('data-filter');
    //            $grid.isotope({
    //                filter: filterValue
    //            });
    //        });
    //        // init Isotope
    //        var $grid = $('.gallery-wrapper').isotope({
    //            itemSelector: '.single-image-gallery',
    //            percentPosition: true,
    //            masonry: {
    //                columnWidth: '.single-image-gallery'
    //            }
    //        });
    //    });
    //}
    //$('.gallery-menu button').on('click', function () {
    //    $('.gallery-menu button').removeClass('active');
    //    $(this).addClass('active');
    //})

    //// :: 21.0 Countdown One

    //if ($.fn.countdown) {
    //    $('#simpleCountdown').countdown('2021/10/10', function (event) {
    //        var $this = $(this).html(event.strftime(
    //            '<span>%D</span> Days ' +
    //            '<span>%H</span> Hour ' +
    //            '<span>%M</span> Min ' +
    //            '<span>%S</span> Sec'));
    //    });
    //}

    //// :: 22.0 Countdown Two

    //if ($.fn.countdown) {
    //    $('#countdown2').countdown('2021/12/9', function (event) {
    //        var $this = $(this).html(event.strftime(
    //            '<span>%D</span> d' +
    //            '<span>%H</span> h' +
    //            '<span>%M</span> m' +
    //            '<span>%S</span> s'));
    //    });
    //}

    //// :: 23.0 Countdown Three

    //if ($.fn.countdown) {
    //    $('#countdown3').countdown('2022/10/10', function (event) {
    //        var $this = $(this).html(event.strftime(
    //            '<span>%D</span> days ' +
    //            '<span>%H</span> : ' +
    //            '<span>%M</span> : ' +
    //            '<span>%S</span>'));
    //    });
    //}

    //// :: 24.0 Counter Up

    //if ($.fn.counterUp) {
    //    $('.counter').counterUp({
    //        delay: 100,
    //        time: 3000
    //    });
    //}

    // :: 25.0 Prevent Default 'a' Click

    $('a[href="#"]').on('click', function ($) {
        $.preventDefault();
    });

    //// :: 26.0 Password Strength

    //if ($.fn.passwordStrength) {
    //    $('#registerPassword').passwordStrength({
    //        minimumChars: 8
    //    });
    //}

    // :: 27.0 Miscellaneous

    $(".form-control, .form-select").on("click", function () {
        $(this).addClass("form-control-clicked");
    })

    $(".active-effect").on("click", function () {
        $(".active-effect").removeClass("active");
        $(this).addClass("active");
    })

    //$(".single-image-gallery .fav-icon").on("click", function () {
    //    $(this).toggleClass("active");
    //})

    //// :: 28.0 ion Range Slider

    //if ($.fn.ionRangeSlider) {
    //    $(".custom-range-slider").ionRangeSlider({});
    //}

    //// :: 29.0 Data Table

    //if ($.fn.DataTable) {
    //    $("#dataTable").DataTable({
    //        "paging": true,
    //        "ordering": true,
    //        "info": true
    //    });
    //}

    //$("#dataTable_length select").addClass("form-select form-select-sm");
    //$("#dataTable_filter input").addClass("form-control form-control-sm");

    //// :: 30.0 Price Table

    //$(".single-price-table").on("click", function () {
    //    $(".single-price-table").removeClass("active");
    //    $(this).addClass("active");
    //});

    // :: 31.0 Tooltip

    var affanTooltip = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = affanTooltip.map(function (tooltip) {
        return new bootstrap.Tooltip(tooltip);
    });

    //// :: 32.0 Toast

    //var affanToast = [].slice.call(document.querySelectorAll('.toast'));
    //var toastList = affanToast.map(function (toast) {
    //    return new bootstrap.Toast(toast);
    //});
    //toastList.forEach(toast => toast.show());

    //$('#toast-showing-btn').on('click', function () {
    //    var affanToast = [].slice.call(document.querySelectorAll('.toast'));
    //    var toastList = affanToast.map(function (toast) {
    //        return new bootstrap.Toast(toast);
    //    });
    //    toastList.forEach(toast => toast.show());
    //});

    //var toastDataDelay = $('.toast-autohide').attr("data-bs-delay");
    //var toastAnimDelay = toastDataDelay + "ms";
    //$(".toast-autohide").append("<span class='toast-autohide-line-animation'></span>");
    //$(".toast-autohide-line-animation").css("animation-duration", toastAnimDelay);

    // :: 33.0 WOW

    //if ($.fn.init) {
    //    new WOW().init();
    //}

})(jQuery);