import React, { Component } from 'react';

export class Inventory extends Component {
  static displayName = Inventory.name;

  constructor(props) {
    super(props);
    this.state = { items: [], loading: true };

    fetch("api/Inventory/Items")
      .then(response => response.json())
      .then(data => {
        this.setState({ items: data.items, loading: false });
      });
  }

  static renderInventoryTable(items) {
    return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th>Symbol</th>
            <th>Category</th>
            <th>Description</th>
            <th>Quantity</th>
          </tr>
        </thead>
        <tbody>
          {items.map(item =>
            <tr key={item.symbol}>
              <td>{item.symbol}</td>
              <td>{item.category}</td>
              <td>{item.description}</td>
              <td>{item.quantity}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Inventory.renderInventoryTable(this.state.items);
    return (
      <div>
        {contents}
      </div>
    );
  }
}