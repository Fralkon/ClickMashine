var surf_cl = document.querySelectorAll('.work-serf');
var n = 1;

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

    var link = surf_cl[n].querySelector('a');
    if (link) {
        link.click();
        return STATUS_OK;
    }

    return STATUS_CONTINUE;
}