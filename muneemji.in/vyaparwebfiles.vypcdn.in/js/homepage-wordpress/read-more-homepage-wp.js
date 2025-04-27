(function($) {
    $.fn.readMore = function(options) {
        var defaults = {
            readMoreLinkClass: "read-more__link",
            readMoreText: "+ Read more",
            readLessText: "- Read less",
            readMoreHeight: 72
        };
        options = $.extend(defaults, options);
        var obj = $(this);

        function getRefElementOptions(refElement) {
            if (typeof refElement.data("options") !== "undefined") {
                this.collapsedHeight = refElement.data("options");
            } else {
                this.collapsedHeight = options.readMoreHeight;
            }
        }

        function addReadMoreElement(element) {
            element.each(function() {
                var $target = $(this);
                var refElementOptions = new getRefElementOptions($target);
                $(this).after("<span>" + options.readMoreText + "</span>").next().addClass(options.readMoreLinkClass);
                $(this).css({
                    "height": refElementOptions.collapsedHeight,
                    "overflow": "hidden"
                });
            });
        }
        addReadMoreElement(obj);
        $("." + options.readMoreLinkClass).click(function() {
            var $target = $(this).prev();
            var refElementOptions = new getRefElementOptions($target);
            if ($target.css("overflow") === "hidden") {
                $target.css({
                    "height": "auto",
                    "overflow": "auto"
                });
                $target.addClass("expanded");
            } else {
                $target.css({
                    "height": refElementOptions.collapsedHeight,
                    "overflow": "hidden"
                });
                $target.removeClass("expanded");
                $target[0].previousElementSibling.scrollIntoView({
                    behavior: "smooth"
                });
            }
            if ($(this).text() === options.readMoreText) {
                $(this).text(options.readLessText);
            } else {
                $(this).text(options.readMoreText);
            }
        });
    };
})(jQuery);
jQuery(document).ready(function($) {
    $('.read-more').readMore();
});