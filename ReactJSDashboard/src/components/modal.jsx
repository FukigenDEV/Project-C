import React, { useState } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

const AdminModal = (props) => {
  const {
    buttonLabel,
    className,
    modalTitle,
    data,
    id,
  } = props;

  const [modal, setModal] = useState(false);

  const toggle = () => setModal(!modal);

  const listData = () => {
    const userlist = [];
    Object.keys(data[id]).forEach(item => {
      if(item !== 'Permissions') {
        userlist.push({key: item, value: data[id][item]});
      } else {
        let permissions = '';
        Object.keys(data[id]['Permissions']).forEach(item => {permissions = permissions + item + ': ' + data[id]['Permissions'][item] + ' - ';});
        userlist.push({key: item, value: permissions});
      }
    });
    return userlist;
  }

  return (
    <div>
      {console.log(`data: ${data}`)}
      <a href onClick={toggle}>{buttonLabel}</a>
      <Modal isOpen={modal} toggle={toggle} className={className}>
        <ModalHeader toggle={toggle}>{modalTitle}</ModalHeader>
        <ModalBody>
          <table>
            {listData().map(row => (
              <tr><td>{row.key}</td><td>{row.value}</td></tr>
            ))}
          </table>
        </ModalBody>
        {/* <ModalFooter>
          <Button color="primary" onClick={toggle}>Do Something</Button>{' '}
          <Button color="secondary" onClick={toggle}>Cancel</Button>
        </ModalFooter> */}
      </Modal>
    </div>
  );
}

export default AdminModal;