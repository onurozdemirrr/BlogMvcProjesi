$(function () {
    // sayfa yüklendikten sonra ilk olarak data-note-id attribute'ne sdahip olan tüm elementleri getirmem gerekecek..
    var noteids = [];           // boş bir dizi tanımladım..

    // div olan ve data-note-id attribute'ü olan elementleri seçiyorum.("div[data-note-id]")
    //each fonksiyonu ile de her bir elemnti geziyorum.
    $("div[data-note-id]").each(function (i, e) {

        // push metodu ile elde ettiğim verileri noteids isimli diziye ekliyorum. $(e). ile elementi seçiyorum. data ile de note-id'li veriye ulaşmış oluyoruym.
        noteids.push($(e).data("note-id"));
        //console.log(noteids);
    });
    $.ajax({
        method: "POST",
        url: "/Note/GetLiked",
        data: { ids: noteids }
    }).done(function (data) {       // Actiondan geriye bir veri gelmesi gerekiyor.. Gelen vEri sisteme login olan user'ın beğendiği notların listesi olacak. data ile bu listeyi buradan alacağım.
        if (data.result != null && data.result.length > 0) {
            for (var i = 0; i < data.result.length; i++) {
                var id = data.result[i];   // beğenmiş olduğum notun id'sini almış oldum.
                var likedNote = $("div[data-note-id=" + id + "]");
                // div[data-note-id=55]

                var btn = likedNote.find("button[data-liked]");

                var span = btn.find("span.like-heart");

                btn.data("liked", true);
                span.removeClass("glyphicon-heart-empty");
                span.addClass("glyphicon-heart");
            }

        }
    }).fail(function () { });

    // Beğenilmemiş bir gönderi olduğunda ve biz beğeni butonuna tıkladığımızda 1 - Veritabanında Likes tablosuna ilgili kayır girilmeli.
    // Beğenilen bir kayıt gönderi için aynı butona tıklandığında da ilgili kayıt veritabanından silinmeli.

    // Sayfa yüklendikten sonra data-liked attribute'ü olan butonlardan hangisine tıklandıysa click(). aşağıdaki metot bunun için çalışacak.
    $("button[data-liked]").click(function () {
        // data-liked değeri true ise gönderi beğenilmiş. false ise gönderi beğenilmemiştir.
        // önce butonu buluyorum.
        var btn = $(this);      // o anki butonu btn değişkenine atıyorum.

        var liked = btn.data("liked");  // true ya da false değerini alıyorum.
        var noteid = btn.data("note-id");   // gönderi beğenildiyse beğenilmedi yapacak. Beğenilmediyse beğenildi yapacak.

        // İkonlarını ve like sayılarını değiştireceğim spanleri buluyorum.
        var spanHeart = btn.find("span.like-heart");
        var spanCount = btn.find("span.like-count");

        // Gerekli bilgileri sayfan toparladım.

        $.ajax({
            method: "POST",
            url: "/Note/SetNoteLike/",
            data: { "noteid": noteid, "liked": !liked }  // noteid'yi ve liked'ın olması gereken değerini Action'a gönderiyorum.
        }).done(function (data) {   // Action'dan gelen sonucu data ile alıyorum.
            if (data.hasError)      // datanın içindeki hasError'e göre; true ise hata var kullanıcı uyarılıyor.
            {
                alert(data.errorMessage);
            }
            else {
                //  false ise  sayfadaki liked değerini değiştiriyorum.
                liked = !liked
                btn.data("liked", liked);
                spanCount.text(data.result);    // Action'dan gelen beğeni sayısını ilgili yere yazdırıyorum.

                // Beğeni butonundaki ikonları/classları kaldırıyorum.
                spanHeart.removeClass("glyphicon-heart-empty");
                spanHeart.removeClass("glyphicon-heart");

                // Aşağıdaki if bloğunda beğenilme ya da beğenilmeyi kaldırma işlemine göre ikonu değiştiriyorum.
                if (liked) {
                    spanHeart.addClass("glyphicon-heart");
                }
                else {
                    spanHeart.addClass("glyphicon-heart-empty");
                }
            }

        }).fail(function () {
            // Giriş yapılmadığında fakat beğeni yapıldığında aşağıdaki uyarıyı veriyor.
            alert("Gönderiyi beğenmek için sisteme giriş yapmalısınız.");
        });
    })
});