$(function () {
    var dataTable = $('.dataTable');
    dataTable.find('.check-all').click(function () {
        var me = this;

        dataTable.find('.id-chekcbox').each(function () {
            this.checked = me.checked;
        });
    });

    $('*[ask]').live('click', function () {
        var me = $(this);

        return confirm(me.attr('ask'));
    });
});

(function ($, Q) {
    Q.media = function (_opt) {

        var opt = $.extend(true, {
            ok: function () { },    //单击“确认”按钮后的回调函数
            single: false,   //是否是单选，false表示多选
            mimetype: ''   //文件类型
        }, _opt);

        var dlg = $('#media-dialog'),
            LAST_SELECTED_EL = null,
            medias = []; //最后被选中的元素

        $(window).resize(function () {
            resizeMediaHeight();
        });

        //loadding mask
        function loading(action) {
            var template = '<div id="loading_mask" style="display:none;">\
                        <div class="mask-layer" style="z-index:10000"></div>\
                        <div class="mask-content mask-loading" style="z-index:10001">\
                            {0}\
                        </div>\
                    </div>';

            content = ' <div class="progress progress-striped active">\
                            <div class="progress-bar" style="width: 60%;"></div>\
                        </div>'

            template = Q.format(template, content);

            var instance = $('#loading_mask'),
                doc = $(window.document),
                win = $(window),
                maskContent = instance.find('.mask-content');

            function layout() {
                instance.find('.mask-layer').css('width', doc.width()).css('height', doc.height());
                maskContent.css('left', (win.width() - maskContent.width() - 200) / 2)
                           .css('top', ((win.height() - maskContent.height()) / 2) + win.scrollTop());
            }

            switch (action) {
                case 'show':
                    if (instance.length < 1) {
                        $(document.body).append(template);
                        instance = $('#loading_mask');
                        maskContent = instance.find('.mask-content');
                        $(window).scroll(function () {
                            layout();
                        });
                        $(window).resize(function () {
                            layout();
                        });
                    }
                    else {
                        maskContent.html(content);
                    }

                    instance.show();
                    layout();

                    break;

                case 'hide':
                    instance.hide();
                    break;
            }
        }

        function initEvents() {
            $('#media-dialog-ok').unbind('click');
            $('#media-dialog-ok').bind('click', function () {
                dlg.modal('hide');
                if (opt.single) {
                    var m = null;
                    if (medias.length > 0) {
                        m = medias[0];
                    }
                    opt.ok(m);
                }
                else {
                    opt.ok(medias);
                }
            });

            $('.media-item').unbind('click');
            $('.media-item').bind('click', function (e) {
                autoselect($(this), e);
            });

            $('.media-item-icon').unbind('mouseover');
            $('.media-item-icon').bind('mouseover', function () {
                var me = $(this);
                me.find('.fa').removeClass('fa-check').addClass('fa-times');
            });

            $('.media-item-icon').unbind('mouseout');
            $('.media-item-icon').bind('mouseout', function () {
                var me = $(this);
                me.find('.fa').removeClass('fa-times').addClass('fa-check');
            });

            $('.media-item-icon').unbind('click');
            $('.media-item-icon').bind('click', function (e) {
                e.stopPropagation();
                unselect($(this).parents('.media-item'));
            });
        }

        function initUpload() {
            var uploader = WebUploader.create({
                pick: {
                    id: '#filePicker',
                    label: '点此选择文件'
                },
                dnd: '#dndArea',
                paste: document.body,
                swf: basepath + 'Scripts/webuploader/Uploader.swf',
                chunked: false,
                chunkSize: 512 * 1024,
                server: mediaupload,
                auto: true,
                disableGlobalDnd: true,
                fileNumLimit: 300,
                fileSizeLimit: 200 * 1024 * 1024,    // 200 M
                fileSingleSizeLimit: 50 * 1024 * 1024    // 50 M
            });

            uploader.on('uploadStart', function (files) {
                var bar = $('#statusBar');
                bar.find('.progress-bar').css('width', '0px');
                bar.show();
            });

            uploader.on('uploadComplete', function () {
                var info = $('#statusInfo'),
                    sts = this.getStats();

                info.html('已上传' + sts.successNum + '个文件，' + sts.uploadFailNum + '个失败，剩余' + sts.queueNum + '个文件。');
            });

            uploader.on('uploadFinished', function () {
                initPager();
                loadMedias(1, false);
            });

            uploader.on('uploadProgress', function (file, percentage) {
                var bar = $('#statusBar');
                bar.find('.progress-bar').css('width', (percentage * 100) + '%');
            });
        }

        function resizeMediaHeight() {
            $(".tile").height($($(".tile")[0]).width());
        }

        function showPager(isshow) {
            if (isshow) {
                $('#pagerbox').show();
            }
            else {
                $('#pagerbox').hide();
            }
        }

        function initPager() {
            var pagecount = parseInt($('#media-dialog-pagecount').val());
            if (!pagecount) {
                pagecount = 0;
            }
            if (pagecount > 0) {
                var options = {
                    bootstrapMajorVersion: 3,
                    currentPage: 1,
                    totalPages: pagecount,
                    pageUrl: function () {
                        return 'javascript:;';
                    },
                    onPageChanged: function (event, oldPage, newPage) {
                        loadMedias(newPage, true);
                    }
                };

                $('#medialog-pager').bootstrapPaginator(options);
            }
        }

        function loadMedias(pageindex, showLoding, _call) {
            _call = _call || function () { };

            if (showLoding) {
                loading('show');
            }

            $('#site-medias').load(mediasurl + '?pageindex=' + pageindex + '&mimetype=' + opt.mimetype, function () {
                loading('hide');

                _call();
                resizeMediaHeight();
                initEvents();
            });
        }

        function addMedia(media) {
            var flag = true;
            for (var i = 0; i < medias.length; i++) {
                if (medias[i].id == media.id) {
                    flag = false;
                    break;
                }
            }

            if (flag) {
                medias.push(media);
            }
        }

        function removeMedia(media) {
            var id = null;
            if (typeof (media) == 'string') {
                id = media;
            }
            else {
                id = media.id;
            }
            for (var i = 0; i < medias.length; ) {
                if (medias[i].id == id) {
                    medias = medias.splice(i + 1, 1);
                    break;
                }
                else {
                    i++;
                }
            }
        }

        function clearMedia() {
            medias = [];
        }

        function toMedia(el) {
            el = $(el);

            var me = {
                url: el.data('url'),
                id: el.data('id'),
                mimetype: el.data('mimetype')
            };

            return me;
        }

        function select(el) {
            el = $(el);

            el.addClass('media-item-selelcted');

            addMedia(toMedia(el));
        }

        function unselect(el) {
            $(el).removeClass('media-item-selelcted');

            var media =  toMedia(el);

            removeMedia(media);
        }

        function unselectall() {
            $('.media-item-selelcted').removeClass('media-item-selelcted');

            clearMedia();
        }

        function isselected(el) {
            return $(el).hasClass('media-item-selelcted');
        }

        function autoselect(el, e) {
            if (!opt.single) {
                //如果按了shift则进行连续选择，如果安了ctrl键，则进行跳跃式选择，否则清除其他选择
                var rote = e.ctrlKey ? 'ctrl' : e.shiftKey ? 'shift' : 'one';

                switch (rote) {
                    case 'ctrl':
                        select(el);
                        LAST_SELECTED_EL = el;
                        break;

                    case 'shift':
                        if (LAST_SELECTED_EL) {
                            var lastIndex = parseInt(LAST_SELECTED_EL.attr('index'));
                            var thisIndex = parseInt(el.attr('index'));
                            var list = null;

                            if (thisIndex > lastIndex) {
                                list = el.prevUntil('div[index="' + lastIndex + '"]');
                            }
                            else {
                                list = el.nextUntil('div[index="' + lastIndex + '"]');
                            }

                            unselectall();

                            list.each(function (i, o) {
                                select(o);
                            });
                            select(LAST_SELECTED_EL);
                            select(el);
                        }
                        else {
                            select(el);
                            LAST_SELECTED_EL = el;
                        }
                        break;

                    default:
                        if (isselected(el)) {
                            unselect(el);
                            LAST_SELECTED_EL = null;
                        }
                        else {
                            unselectall();
                            select(el);
                            LAST_SELECTED_EL = el;
                        }

                        break;
                }
            }
            else {
                unselectall();
                select(el);
            }
        }

        function show() {
            if (opt.single) {
                $('#media-single').show();
            }
            else {
                $('#media-single').hide();
            }
            
            unselectall();
            dlg.modal();
        }

        if (dlg.length < 1) {
            loading('show');

            $('<div></div>').appendTo(document.body).load(medialogurl, function () {
                loadMedias(1, true, function () {
                    dlg = $('#media-dialog');

                    $('#media-tabs a').on('shown.bs.tab', function (e) {
                        var me = $(e.target);

                        showPager(false);

                        if (me.attr('href') == '#local-media-upload') {
                            initUpload();
                        }
                        else if (me.attr('href') == '#site-medias') {
                            resizeMediaHeight();
                            showPager(true);
                        }
                    });

                    dlg.on('shown.bs.modal', function (e) {
                        resizeMediaHeight();
                    });

                    $('#input-media-url').change(function () {
                        var me = $(this),
                            val = me.val();
                        var r = val.toLowerCase().split('.');
                        switch (r[r.length - 1]) {
                            case 'jpg':
                            case 'png':
                            case 'gif':
                            case 'jepg':
                            case 'bmp':
                                $('#remote-pic-box').html('<img src="' + val + '" class="img-responsive" />');
                                break;
                        }
                    });

                    initPager();
                    showPager(true);
                    show();
                });
            });
        }
        else {
            initEvents();
            show();
        }
    };

    //编辑器
    Q.editor = function (el, opt) {
        opt = $.extend(true, {}, opt);
        var ue = UE.getEditor(el, opt);

        return ue;
    }
}($, Q, window));