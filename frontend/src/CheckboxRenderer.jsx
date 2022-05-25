function CheckboxRenderer(props) {
  function checkedHandler(event) {
    let checked = event.target.checked;
    let colId = props.column.colId;
    var currentList = JSON.parse(localStorage.getItem("mylist"));
    if (currentList == null) {
      currentList = [];
    }
    if (checked) {
      currentList.push(props.node.data.id);
    }
    else {
      currentList = currentList.filter(x => x !== props.node.data.id);
    }
    localStorage.setItem("mylist", JSON.stringify(currentList));
    props.node.setDataValue(colId, checked);
    props.callbackSelectedList(currentList);
  }
  return <input
    type="checkbox"
    onChange={checkedHandler}
    checked={props.value}
  />
}

export default CheckboxRenderer;