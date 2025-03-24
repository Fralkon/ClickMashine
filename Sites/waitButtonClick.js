function wait_element() {
    var element = {0}
    if (element != null) { element.click(); return STATUS_OK; }
    else return STATUS_WAIT;
}