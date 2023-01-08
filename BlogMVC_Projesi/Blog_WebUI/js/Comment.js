
modelComponentBodyId = '#modal_yorum_body';
var noteId = -1;

$(function () {
    $('#modal_yorum').on('show.bs.modal', function (e) {
        var btn = $(e.relatedTarget);
        noteId = btn.data("note-id");
        $('#modal_yorum_body').load('/Comment/ShowNoteComments/' + noteId)
    })
});

function doComment(btn, eventMode, commentId, spanId) {
    var button = $(btn);    // parametreden gelen btn nesnesini JQuery'de kullanıma hazırladım.
    var mode = button.data("edit-mode");    // partialdaki button içindeki data-edit-mode özelliğinin değerini alıyorum.
    if (eventMode === "edit-clicked") {
        if (!mode)      // mode false ise aşağıdaki işlemleri yapacak.
        {
            button.data("edit-mode", true);     //data-edit-mode = true yapıyorum. Burası güncellenebilir bilgisi için
            button.removeClass("btn-warning");  // Butonun rengini kaldırıyorum.
            button.addClass("btn-success");     // Butonun rengini yeşil yapıyorum

            var btnSpan = button.find("span");  // Butonun içindeki span'i bulup btnSpan değişkenine aktarıyorum.
            btnSpan.removeClass("glyphicon-edit");  // Span'in içindeki edit'i kaldırıyorum.
            btnSpan.addClass("glyphicon-ok");   // ok ikonunu ekliyorum.

            // doComment fonksiyonuna parametre olarak gönderilen spanId'si yani partial içindeki yorumun yazıldığı Span. comment.Text'in yazıldığı satıra ait olan Span.
            $(spanId).attr("contenteditable", true);    //İlgili spanin conteneditable özelliğini true yapıyorum. Böylece bu span edit edilebilir textbox'a dönüşecek.
            $(spanId).addClass("editable");  //İlgili span'a editable isminde bir class ekliyorum. Css ekleyip bir takım özelliklerini değiştireceğim.
            $(spanId).focus();  // span edit edilebilir hale geldikten sonra yani textbox olduktan sonra cursor burada konumlansın diye yazdığım satır.
        }
        else  // mode true ise yukarıdaki işlemlerin tam tersini yapacak.
        {
            button.data("edit-mode", false);
            button.removeClass("btn-success");
            button.addClass("btn-warning");

            var btnSpan = button.find("span");
            btnSpan.removeClass("glyphicon-ok");
            btnSpan.addClass("glyphicon-edit");

            $(spanId).attr("contenteditable", false);
            $(spanId).removeClass("editable");

            // Burada edit edilen yorumu veritabanına kaydetmemiz gereken kodları yazacağız. Değişikliği ilgili Action ve controller'a göndereceğiz.

            var txt = $(spanId).text();     // ilgili span içinden text değerini yani değiştirilmiş yorumu alıyoruz.
            $.ajax({
                method: "POST",
                url: "/Comment/Edit/" + commentId,
                data: { text: txt }
            }).done(function (data) {   // ajax metodu sonucu başarılı olursa burası yani done kısmı çalışacak
                if (data.result) {

                    $(modelComponentBodyId).load('/Comment/ShowNoteComments/' + noteId);

                } else {
                    // güncelleme yapılamaış oluyor.. burada da kullanıcıya mesaj vereceğiz.
                    alert("Yorum güncellenemedi.");
                }


            }).fail(function () {   // ajax metodu sonucu başarısız olursa burası yani fail kısmı çalışacak
                // Action ile ilgili bir problem olduğunda çalışacak bölüm.
                alert("Sunucuya bağlanamadı.");

            });
        }
    }

    else if (eventMode === 'delete-clicked') {
        // Kullanıcı silme butonuna bastığında bir onay almamız gerekecek.
        var dialogResult = confirm("Yorum silinsin mi?")
        if (!dialogResult) return false;    // Silme işlemi iptal edilecek.


        $.ajax({
            method: "GET",
            url: "/Comment/Delete/" + commentId
        }).done(function (data) {   // ajax metodu sonucu başarılı olursa burası yani done kısmı çalışacak
            // data ile Action'dan bir değer gelecek true ya da false. true: silme işlemi başarılı. false: silme işlemi başarısız.
            if (data) {
                // Silme işlemi başarılı ise yorumlar sayfasını güncellemesi gerekiyor. Yani silinen yorum ekrana gelmeyecek.
                $(modelComponentBodyId).load('/Comment/ShowNoteComments/' + noteId);    // Yorumlar bölümünü tekrardan yükleyecek.
            }
            else {
                alert("Yorum silinemedi.")
            }


        }).fail(function () {   // ajax metodu sonucu başarısız olursa burası yani fail kısmı çalışacak
            // Action ile ilgili bir problem olduğunda çalışacak bölüm.
            alert("Sunucuya bağlanamadı. Silme işlemi iptal edildi.");

        });
    }
    else if (eventMode === 'new-clicked') {
        // Eklenen yorumuj ilgili input'un textinden alacağım ve Insert Action'ına göndermem gerekecek. Veritabanına kaydetmek için.

        var txt = $('#new_comment_text').val(); // input içindeki veriyi almak için yazdığımız kod.

        $.ajax({
            method: "POST",
            url: "/Comment/Create/",
            data: { "Text": txt, "noteId": noteId }     // "Text" şeklinde çift tırnak içinde yazdığımda değişken adı olarak algılanacak.

        }).done(function (data) {   // ajax metodu sonucu başarılı olursa burası yani done kısmı çalışacak
            // data ile Action'dan bir değer gelecek true ya da false. true: silme işlemi başarılı. false: silme işlemi başarısız.

            if (data) {
                // _PartialComment ile verileri tekrardan yüklüyorum.
                $(modelComponentBodyId).load('/Comment/ShowNoteComments/' + noteId);    // Yorumlar bölümünü tekrardan yükleyecek.
            }
            else {
                alert("Yorum eklenemedi.")
            }


        }).fail(function () {   // ajax metodu sonucu başarısız olursa burası yani fail kısmı çalışacak
            // Action ile ilgili bir problem olduğunda çalışacak bölüm.
            alert("Sunucuya bağlanamadı. Yorum ekleme işlemi iptal edildi.");

        });
    }
}