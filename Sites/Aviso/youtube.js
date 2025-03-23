var surf_cl = document.querySelectorAll('.work-serf');
var n = 0;

function FirstStep() {
    if (n >= surf_cl.length) return STATUS_END;

    var id = surf_cl[n].id.charAt(0);

    switch (id) {
        case 'p':
        case 'l':
        case undefined:
            break;
        case 'a':
            surf_cl[n].querySelector('span').click();
            return STATUS_OK1;
    }

    n++;
    return STATUS_CONTINUE;
}