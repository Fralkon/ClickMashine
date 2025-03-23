function get_mail() {
    var mail_text = {0};
    if (mail_text != null) {
        var answer = {1};
        var quest = {2};
        return JSON.stringify(
            {
                Mail: mail_text.innerText,
                Question: quest.innerText,
                Answer: [answer[0].innerText,
                answer[1].innerText,
                answer[2].innerText]
            });
    }
    else { return STATUS_WAIT; }
};
get_mail();