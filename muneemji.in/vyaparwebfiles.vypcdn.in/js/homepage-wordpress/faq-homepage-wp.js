$(document).ready(function() {
    $('.up-arrow').hide();
    $('.faq-question').on('click', function() {
        const $allAnswers = $('.faq-answer');
        const $allQuestions = $('.faq-question');
        const $this = $(this);
        const $answer = $this.next('.faq-answer');

        if (!$answer.hasClass('show-faq')) {
            // Hide all answers
            $allAnswers.removeClass('show-faq').slideUp(1000);
            $allQuestions.find('.up-arrow').hide();
            $allQuestions.find('.down-arrow').show();
            // Show the clicked answer
            $answer.addClass('show-faq').slideDown(1000);
            $this.find('.down-arrow').hide();
            $this.find('.up-arrow').show();
        } else {
            // Hide the clicked answer if it's already open
            $answer.removeClass('show-faq').slideUp(1000);
            $this.find('.down-arrow').show();
            $this.find('.up-arrow').hide();
        }
    });
    $('.owl-dot').click(function() {
        $('.owl-dot').trigger('to.owl.carousel', [$(this).index(), 100]);
    })
    $('.faq-question:first').click();
});