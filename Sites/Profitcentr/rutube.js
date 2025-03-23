var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function click_s() {
	if (n >= surf_cl.length) return STATUS_END;
	else {
		surf_cl[n].querySelector('span').click(); return STATUS_OK;
	}
}
function surf() {
	var start_ln = surf_cl[n].querySelector('.youtube-button');
	if (start_ln != null) {
		if (start_ln.innerText != 'Приступить к просмотру') {
			n++; return STATUS_CONTINUE; }
		else { start_ln.querySelector('span').click(); n++; return STATUS_OK; }
	}
	else { return STATUS_END; }
}