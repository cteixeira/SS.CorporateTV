

function RenderConteudoVideo(videos) {
    if (videos[0].substring(videos[0].lastIndexOf('.') + 1, videos[0].length) == 'mp4') {
        CriarVideo(videos);
    }
    else {
        var ua = window.navigator.userAgent;
        var msie = ua.indexOf("MSIE ");
        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
            CriarObject(videos);
            setTimeout(function () { $('#objectid').attr('style', 'width:100%; height:100%') }, 30);
            setTimeout(function () { $('#objectid').hide() }, 40);
            setTimeout(function () { $('#objectid').show() }, 60);
        }
        else {
            CriarEmbed(videos);
        }
    }
}

function RenderConteudoImagens(imagens, tempos) {
    CriarImagem(imagens, tempos);
    RodarImagens();
}

var xmlhttpstream = null;

function AtualizarStream() {

    if (xmlhttpstream) {
        xmlhttpstream.abort();
    }

    xmlhttpstream = new XMLHttpRequest();


    xmlhttpstream.onreadystatechange = function () {

        if (xmlhttpstream.readyState === 4 && xmlhttpstream.status === 200) {
            if (!xmlhttpstream.response) { return; }
            var jsontv = JSON.parse(xmlhttpstream.response);
            var jsonTvID = jsontv.TvID;
            if (!jsonTvID) {
                window.location.href = ROOT + "/TV/MostrarError";
            }

            var jsonTipoConteudo = jsontv.ConteudoNoAr.TipoConteudoAr;

            if(jsonTipoConteudo != 0)
            {
                clearTimeout(idTimeoutImages);

                if (jsonTipoConteudo == TIPOCONTEUDOVIDEO || jsonTipoConteudo == TIPOCONTEUDOCANAL)
                {
                    var videos = jsontv.ConteudoNoAr.Videos;
                    if (videos != null && videos.length > 0)
                    {
                        RenderConteudoVideo(videos)
                    }
                }
                else if (jsonTipoConteudo == TIPOCONTEUDOIMAGEM)
                {
                    var imagens = jsontv.ConteudoNoAr.Imagens;
                    if (imagens != null && imagens.length > 0)
                    {
                        var tempos = jsontv.ConteudoNoAr.DuracaoImagens;
                        RenderConteudoImagens(imagens, tempos)
                    }
                }
                else
                {
                    CriarImagemDefault();
                }
            }
            else
            {
                CriarImagemDefault();
            }
            agendamentoID = jsontv.ConteudoNoAr.ProgramacaoIDNoAr;
            conteudoID = jsontv.ConteudoNoAr.ConteudoIDNoAr;

            setTimeout(AtualizarStream, jsontv.ConteudoNoAr.TempoParaFim);
        }
    }

    xmlhttpstream.open("GET", ROOT + "/TV/AtualizarStream" + "?agendamentoNoAr=" + agendamentoID + "&conteudoNoAr=" + conteudoID, true);
    xmlhttpstream.send();
}

function CriarEmbed(src) {
    if(src.length)
    {
        var embed = document.createElement('embed');
        embed.setAttribute("id", "embedid");
        embed.setAttribute("type", "application/x-vlc-plugin");
        embed.setAttribute("pluginspage", "http://www.videolan.org");
        embed.setAttribute("autoplay", "yes");
        embed.setAttribute("loop", "yes");
        //embed.setAttribute("target" , "");
        embed.style.width = "100%";
        embed.style.height = "100%";
        AppendHTML(embed, true);

        if($('#embedid').length){
            if(document.getElementById("embedid").video != undefined && document.getElementById("embedid").video != null)
                document.getElementById("embedid").video.aspectRatio = AspectRatioVideoServer;

            if(document.getElementById("embedid").audio != undefined && document.getElementById("embedid").audio != null)
                document.getElementById("embedid").audio.volume = VolumeVideoServer;
        }

        var index, len;
        var options = new Array();
        for (index = 0, len = src.length; index < len; ++index) {
            document.getElementById("embedid").playlist.add(src[index], "Movie" + index , options);
        }
        document.getElementById("embedid").playlist.play();
    }
}

function CriarObject(src) {
    var object = document.createElement('object');
    object.setAttribute("id", "objectid");
    object.setAttribute("classid", "clsid:9BE31822-FDAD-461B-AD51-BE1D1C159921");
    object.setAttribute("style", "width: 100%; height: 100%;");
    var param2 = document.createElement("param");
    param2.setAttribute("name", "AutoLoop");
    param2.setAttribute("value", "True");
    object.appendChild(param2);
    var param3 = document.createElement("param");
    param3.setAttribute("name", "AutoPlay");
    param3.setAttribute("value", "True");
    object.appendChild(param3);
    AppendHTML(object, true);

    var index, len;
    var options = new Array();
    for (index = 0, len = src.length; index < len; ++index) {
        object.playlist.add(src[index]);
    }
    object.playlist.play();
}

function CriarVideo(src) {
    var video = document.createElement('video');
    video.setAttribute("id", "videoid");
    //video.setAttribute("autoplay", true);
    //video.setAttribute("loop", false);
    video.setAttribute("controls", '');
    video.style.width = "100%";//4.45em";

    var source = document.createElement("source");
    source.setAttribute("id", "sourcevideo");
    source.setAttribute("src", src[0]);
    source.setAttribute("type", "video/mp4");
    video.appendChild(source);

    AppendHTML(video, true);

    var activeVideo = 0;
    video = document.getElementById('videoid');
    video.addEventListener('ended', function(e) {
        // update the new active video index
        activeVideo = (++activeVideo) % src.length;

        // update the video source and play
        video.src = src[activeVideo];
        video.play();

        if(activeVideo == src.length)
            activeVideo = 0;
    });
    video.addEventListener('error', function(event)
    {
        activeVideo = (++activeVideo) % src.length;
        video.src = src[activeVideo];
        video.play();

        if(activeVideo == src.length)
            activeVideo = 0;
    }, true);
    video.play();

    if (video.length){
        video.prop("volume", VolumeVideoFile);
    }
}

function CriarImagem(src, times) {
    if (src.length) {
        var index, len;
        var active = "active";
        var first = true;
        for (index = 0, len = src.length; index < len; ++index) {
            var imgSrc = "data:image/gif;base64," + src[index];

            var img = document.createElement('img');
            img.setAttribute("id", "imgid");
            img.setAttribute("src", imgSrc);
            img.setAttribute("width", '100%');
            img.setAttribute("height", 'auto');
            img.setAttribute("class", active);
            img.setAttribute("data-timeout", times[index]);
            AppendHTML(img, first);
            active = ''; first = false;
        }
    }
}

function AppendHTML(obj, isfirst) {
    var iddivvideo = document.getElementById("iddivvideo");
    if (isfirst) {
        while (iddivvideo.firstChild)
            iddivvideo.removeChild(iddivvideo.firstChild);
    }
    iddivvideo.appendChild(obj);
}

var idTimeoutImages;
function RodarImagens()
{
    var $x = $('#iddivvideo');
    var spin = function () {
        var $a = $x.children('.active');
        var t = $a.data('timeout');

        idTimeoutImages = setTimeout(function () {
            console.log('in timeout function');
            $a.removeClass('active');
            var $b = $a.next();
            if ($b.length == 0) {
                $b = $a.siblings().first();
            }
            $b.addClass('active');
            spin();
        }, t);
    }
    spin();
}

function CriarImagemDefault() {
    var img = document.createElement('img');
    img.setAttribute("id", "imgid");
    img.setAttribute("src", ROOT + "/Images/hospital.jpg");
    img.setAttribute("width", '100%');
    img.setAttribute("height", 'auto');
    img.setAttribute("class", "active");
    img.setAttribute("data-timeout", "60000");
    AppendHTML(img, true);
    img = document.createElement('img');
    img.setAttribute("id", "imgid");
    img.setAttribute("src", ROOT + "/Images/hospital.jpg");
    img.setAttribute("width", '100%');
    img.setAttribute("height", 'auto');
    img.setAttribute("class", "");
    img.setAttribute("data-timeout", "60000");
    AppendHTML(img, false);
}

function OnLoadMain() {
    setTimeout(AtualizarStream, intervalofimprograma);
    setInterval(AtualizarStream, intervaloatualizarstream);

    var tipoConteudo = tipoConteudoAr;
    if(tipoConteudo == 'Video' || tipoConteudo == 'Canal')
    {
        var videos = JSON.parse(videosNoAr);
        if (videos != null && videos.length > 0)
        {
            RenderConteudoVideo(videos)
        }
    }
    else  if(tipoConteudo == 'Imagem')
    {
        var imagens = JSON.parse(imagensNoAr);
        if (imagens != null && imagens.length > 0)
        {
            var tempos = JSON.parse(duracaoImagensNoAr);
            RenderConteudoImagens(imagens, tempos)
        }
    }
    else
    {
        CriarImagemDefault();
    }
}