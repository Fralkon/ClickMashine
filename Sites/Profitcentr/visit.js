var surf_cl = document.querySelectorAll('.work-serf');
var n = 0;

function FirstStep() {
    if (n >= surf_cl.length) return STATUS_END;

    var link = surf_cl[n];

    // Проверяем, существует ли третья ячейка и имеет ли элемент высоту
    var cell = link.querySelectorAll('td')[2];
    if (!cell || link.getBoundingClientRect().height === 0) {
        n++;
        return STATUS_CONTINUE;
    }

    var anchor = link.querySelector('a');
    if (anchor) {
        anchor.click();
        n++;
        return STATUS_OK;
    }

    return STATUS_CONTINUE;
}

function SecondStep() {
    return STATUS_OK;
}
