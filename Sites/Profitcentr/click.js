var surf_cl = document.querySelectorAll('.work-serf');
var n = 0;

function SecondStep() {
    var start_ln = surf_cl[n].querySelector('.butt-yes-test');
    if (start_ln) {
        start_ln.click();
        n++;
        return STATUS_OK;
    }
    return STATUS_WAIT;
}

function FirstStep() {
    if (n >= surf_cl.length) return STATUS_END;

    var currentItem = surf_cl[n];
    var workquest = currentItem.querySelector('.workquest');

    if (workquest) {
        var questLink = currentItem.querySelectorAll('td')[2].querySelector('a');
        if (!questLink || currentItem.getBoundingClientRect().height === 0) {
            n++;
            return STATUS_CONTINUE;
        }
        questLink.click();
        return STATUS_OK;
    }

    n++;
    return STATUS_CONTINUE;
}