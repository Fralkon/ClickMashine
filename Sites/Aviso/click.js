var surf_cl = document.querySelectorAll('.work-serf'); var n = 0;
function SecondStep() {
    var start_ln = surf_cl[n].querySelector('.start-yes-serf');
    if (start_ln != null) {
        n++; start_ln.click(); return STATUS_OK; }
    else { return STATUS_WAIT; }
    }
    function FirstStep() {
        if (n >= surf_cl.length) return STATUS_END;
        else if (surf_cl[n].innerText.length > 200) { n++; STATUS_CONTINUE; }
        else {
            surf_cl[n].querySelector('a').click(); return STATUS_OK;
        }
    }