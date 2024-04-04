/// <reference types ="@altv/types-client" />
/// <reference types ="@altv/types-natives" />

import * as alt from 'alt-client';
import * as native from 'natives';

//variablen
let loginHud;
let guiHud;

alt.onServer('freezePlayer', (freeze) => {
    const lPlayer = alt.Player.local.scriptID;
    native.freezeEntityPosition(lPlayer, freeze)
})

alt.onServer("CloseLoginHud", () => {
    alt.showCursor(false);
    alt.toggleGameControls(true);
    alt.toggleVoiceControls(true);

    if (loginHud) {
        loginHud.destroy();
    }
})

alt.onServer("SendErrorMessage", (text) => {
    loginHud.emit("ErrorMessage", text);
})


alt.on('connectionComplete', () => {
    loadBlips();
    guiHud = new alt.WebView("http://resource/gui/gui.html");
    loginHud = new alt.WebView("http://resource/login/login.html");
    loginHud.focus();

    alt.showCursor(true);
    alt.toggleGameControls(false);
    alt.toggleVoiceControls(false);

    loginHud.on('Auth.Login', (name, password) => {
        alt.emitServer('Event.Login', name, password);
    })

    loginHud.on('Auth.Register', (name, password) => {
        alt.emitServer('Event.Register', name, password);
    })
})

alt.onServer("sendNotification", (status, text) => {
    guiHud.emit('sendNotification', status, text);
})


function loadBlips() {
    createBlip(0, 0, 70, 8, 29, 1.0, false, "Zivispawn");
}


function createBlip(x, y, z, sprite, color, scale = 1.0, shortRange = false, name = "") {
    const tempblip = new alt.PointBlip(x, y, z);
    tempblip.sprite = sprite;
    tempblip.color = color;
    tempblip.scale = scale;
    tempblip.shortRange = shortRange;
    if (name.length > 0) {
        tempblip.name = name;
    }
}

//DrawText2D
function drawText2d(
    msg,
    x,
    y,
    scale,
    fontType,
    r,
    g,
    b,
    a,
    useOutline = true,
    useDropShadow = true,
    layer = 0,
    align = 0
) {
    let hex = msg.match('{.*}');
    if (hex) {
        const rgb = hexToRgb(hex[0].replace('{', '').replace('}', ''));
        r = rgb[0];
        g = rgb[1];
        b = rgb[2];
        msg = msg.replace(hex[0], '');
    }

    native.beginTextCommandDisplayText('STRING');
    native.addTextComponentSubstringPlayerName(msg);
    native.setTextFont(fontType);
    native.setTextScale(1, scale);
    native.setTextWrap(0.0, 1.0);
    native.setTextCentre(true);
    native.setTextColour(r, g, b, a);
    native.setTextJustification(align);

    if (useOutline) {
        native.setTextOutline();
    }

    if (useDropShadow) {
        native.setTextDropShadow();
    }

    native.endTextCommandDisplayText(x, y, 0);
}



//speedometer 
alt.everyTick(() => {
    const lPlayer = alt.Player.local;
    let vehicle = lPlayer.vehicle;

    if (vehicle) {
        let speed = vehicle.speed * 3.6;
        speed = Math.round(speed);
        drawText2d(`${speed} KMH`, 0.55, 0.91, 1.5, 2, 255, 255, 255, 255, true);

    }
})
