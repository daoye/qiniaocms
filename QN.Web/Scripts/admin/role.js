(function () {
    $('.tdparent').click(function () {
        var me = $(this),
            cb = me.find('input[type="checkbox"]'),
            childs = me.next('.tdchild').find('input[type="checkbox"]');

        for (var i = 0; i < childs.length; i++) {
            if (cb.get(0).checked) {
                $(childs[i]).removeAttr('disabled');
                childs[i].checked = true;
            }
            else {
                $(childs[i]).attr('disabled', 'disabled');
                childs[i].checked = false;
            }
        }
    });

    function fistState() {
        var p = $('.tdparent');
        for (var i = 0; i < p.length; i++) {
            var me = $(p[i]),
                childs = me.next('.tdchild').find('input[type="checkbox"]');
            for (var j = 0; j < childs.length; j++) {
                if (me.find('input[type="checkbox"]').get(0).checked) {
                    $(childs[j]).removeAttr('disabled');
                }
                else {
                    $(childs[j]).attr('disabled', 'disabled');
                }
            }
        }
    }

    fistState();
}());
