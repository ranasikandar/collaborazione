// Check compatibility for the browser
if ("serviceWorker" in navigator) {

    // Register the service worker
    navigator.serviceWorker
        .register("/serviceworker.js", {
            scope: "/"
        })
        .then(function (reg) {
            //console.log("[PWA Builder] Service worker registered for scope: " + reg.scope);

            //// if pwa to android app notification permission is granted so sub here without user noti sub prompt
            //if (Notification.permission === "granted") {
            //    getSubscription(reg);
            //} else {
            //    $("#PromptForNotificationAccess").click(() => requestNotificationAccess(reg));
            //    requestNotificationAccess(reg);
            //}

            $("#PromptForNotificationAccess").click(() => requestNotificationAccess(reg));
        });
} else {
    //browser doesnt support app install redirect to website https://collaborazione.immobiliare-stella.com/User
    window.setTimeout(function () { window.location.replace("/User"); }, 5000);
    swal("browser doesn't support App installation, redirecting to website", {
        icon: "error",
        timer: 5000,
    });
}

//notification
function requestNotificationAccess(reg) {

    Notification.requestPermission(function (status) {
        if (status == "granted") {
            getSubscription(reg);
        } else {
            swal("il tuo browser non consente di ricevere notifiche, per favore abilita prima la notifica push", {
                icon: "error",
                timer: 3000,
            });
        }
    });
}

function getSubscription(reg) {
    reg.pushManager.getSubscription().then(function (sub) {
        //if (sub === null) {
            reg.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: "BIPtFZ9OQaZYrGIl4deBc_R5xtccebaONW5V4R50XR5L6lJodFuqJpIP2GLCCAsvU4WNyw3MLsJNhogF6dN2cV8"
            }).then(function (sub) {
                SubNotiServerCal(sub);
            }).catch(function (e) {
                console.error("Unable to subscribe to push", e);
                swal("Impossibile iscriversi alla notifica push", {
                    icon: "error",
                    timer: 3000,
                });
            });
        //}

    });
}

function SubNotiServerCal(sub) {

    let subNotiData = {
        "endpoint": sub.endpoint,
        "p256dh": arrayBufferToBase64(sub.getKey("p256dh")),
        "auth": arrayBufferToBase64(sub.getKey("auth"))
    };

    $.ajax(
        {
            url: '/User/SubscribePushNoti',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(subNotiData),
            dataType: 'json',
            headers: {
                RequestVerificationToken:
                    $('input:hidden[name="CSRF_TOKEN_COLLAB_FORM5"]').val()
            },
            async: true,
            success: function (data) {
                console.log(data);
                swal("ti sei iscritto alla notifica push", {
                    icon: "success",
                    timer: 3000,
                });
            },
            error: function (req, status) {
                console.error(status);
                swal("Impossibile iscriversi alla notifica push", {
                    icon: "error",
                    timer: 3000,
                });
            }
        })
}

function arrayBufferToBase64(buffer) {
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}

//notification

//when open pwa enabled website this event is fired
let deferredPrompt;
window.addEventListener('beforeinstallprompt', (e) => {
    e.preventDefault();
    deferredPrompt = e;
    $('#InstallApp').show();
});
//installing

// pwa share
shareData = async () => {
    const data = {
        title: 'Collaborazione',
        text: 'Please check this App',
        url: 'https://collaborazione.immobiliare-stella.com/?fname=' + $('input:hidden[name="currentUsrName"]').val()+'',
    }
    try {
        await navigator.share(data)
    } catch (err) {
        swal({
            title: "Request Failed!",
            text: err,
            icon:"error",
        });
    }
}
// pwa share
