import React, { useState, useEffect } from 'react';
import { AgGridColumn, AgGridReact } from 'ag-grid-react';

import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';

function App() {
    const [rowData, setRowData] = useState([]);
    const [gridApi, setGridApi] = useState(null);
    const [loading, setLoading] = useState(false);
    const [songs, setSongs] = useState([]);
    const [catalogs, setCatalogs] = useState([]);
    const [categories, setCategories] = useState([]);
    const [category, setCategory] = useState(undefined);
    const [catalogName, setCatalogName] = useState(undefined);
    const searchDivStyle = { backgroundColor: "#dedede", padding: 10 }
    const searchStyle = {
        width: "100%", padding: "10px 20px", borderRadius: 20, outline: 0,
        border: "2px #68bf40 solid", fontSize: "100%"
    }

    const onGridReady = (params) => {
        setGridApi(params.api);
    };
    const onFilterTextChange = (e) => {
        gridApi.setQuickFilter(e.target.value)
    }
    useEffect(() => {
        fetchSongsRemotely();
    }, []);
    function getLoading() {
        if (loading) {
            return <div className="spinner-border" role="status">
                <span className="visually-hidden">Loading...</span>
            </div>
        }
    }
    function getCategories() {
        return <div className="col">
            <select className="form-select" name='catalog' onChange={onCategoryChanged}>
                <option value=""> Select category</option>
                {categories.map(key =>
                    (<option value={key} key={key}>{key}</option>))}
            </select>
        </div>
    }
    function getSearchBar() {
        if (rowData.length > 0) {
            return <div style={searchDivStyle}>
                <input type="search" style={searchStyle} onChange={onFilterTextChange} placeholder="search songs..." />
            </div>
        }
    }
    function fetchSongsRemotely() {
        setLoading(true);
        fetch('https://karaoke-juliane.herokuapp.com/songs')
            .then(result => {
                var test = result.json();
                return test;
            })
            .then(response => {
                setLoading(false);
                let fullCatalog = response.songGroups;
                let catalogs = fullCatalog.flatMap(x => x.catalogs).filter((v, i, a) => a.indexOf(v) === i && v != null);
                let categories = fullCatalog.flatMap(x => x.categories).filter((v, i, a) => a.indexOf(v) === i && v != null);
                setCatalogs(catalogs);
                setCategories(categories);
                setSongs(response.songGroups);
                setRowData(fullCatalog);
            });
    }

    function getRowData(newCatalogName, newCategory)
    {
        let result = newCatalogName == null || newCatalogName === '' ? songs : songs.filter(x => x.catalogs.includes(newCatalogName));
        if(newCategory != null && newCategory !== '')
        {
            result = result.filter(x =>x.categories.includes(newCategory));
        }
        return result;
    }

    function onCategoryChanged(event) {
        let newCategory = event.target.value === '' ? undefined : event.target.value;
        setCategory(newCategory);
        setRowData(getRowData(catalogName, newCategory));
    }

    function onCatalogChanged(event) {
        let newCatalog = event.target.value === '' ? undefined : event.target.value;
        setCatalogName(newCatalog);
        setRowData(getRowData(newCatalog, category));
    }

    return (
        <div>
            <h1 align="center">Karaoke night</h1>
            <div className="row">
                <div className="col">
                    <select className="form-select" name='catalog' onChange={onCatalogChanged}>
                        <option value=""> Select catalog</option>
                        {catalogs.map(key =>
                            (<option value={key} key={key}>{key}</option>))}
                    </select>
                </div>
                {getCategories()}
                <div className="col">
                    <button onClick={fetchSongsRemotely} className="btn btn-primary">
                        Reload Songs
                    </button>
                    {getLoading()}
                </div>
            </div>
            {getSearchBar()}


            <div className="ag-theme-alpine"  style={{ "height": 1000 }}>
                <AgGridReact rowData={rowData}
                    defaultColDef={{
                        sortable: true,
                        resizable: true,
                        filter: true,
                        suppressMovable: true,
                        suppressColumnsToolPanel: false
                    }}
                    onGridReady={onGridReady}>
                    <AgGridColumn field="number" sortable={true} filter={true} flex={1} minWidth={80}/>
                    <AgGridColumn field="name" sortable={true} filter={true} flex={4} minWidth={250}/>
                    <AgGridColumn field="artist" sortable={true} filter={true} flex={2} sort={'asc'} minWidth={170}/>
                </AgGridReact>
            </div>
        </div>
    );
};


export default App;