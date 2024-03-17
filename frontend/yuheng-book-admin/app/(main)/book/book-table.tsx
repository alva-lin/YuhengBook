import { ActionIcon, Checkbox, Table } from '@mantine/core';
import { notifications } from '@mantine/notifications';
import cx from 'clsx';
import { useState } from 'react';
import { BookInfoDto } from '@/lib/models';
import classes from './book-table.module.css';

export function BookTable({ data }: { data: BookInfoDto[] }) {
  const [selection, setSelection] = useState([1]);
  const toggleRow = (id: number) =>
    setSelection((current) =>
      current.includes(id) ? current.filter((item) => item !== id) : [...current, id]
    );
  const toggleAll = () =>
    setSelection((current) => (current.length === data.length ? [] : data.map((item) => item.id)));

  const editItem = (id: number) => {
    notifications.show({
      color: 'green',
      title: 'you clicked edit',
      message: `id: ${id}`,
    });
  };

  const removeItem = (id: number) => {
    notifications.show({
      title: 'you clicked remove',
      message: `id: ${id}`,
    });
  };

  const rows = data.map((item) => {
    const selected = selection.includes(item.id);
    return (
      <Table.Tr key={item.id} className={cx({ [classes.rowSelected]: selected })}>
        <Table.Td>
          <Checkbox checked={selection.includes(item.id)} onChange={() => toggleRow(item.id)} />
        </Table.Td>
        <Table.Td>
          {item.name} ({item.chapterCount})
        </Table.Td>
        <Table.Td>{item.lastChapter ? item.lastChapter.title : '-'}</Table.Td>
        <Table.Td>
          <div className="flex gap-4">
            <ActionIcon
              variant="default"
              size="lg"
              radius="sm"
              onClick={() => {
                editItem(item.id);
              }}
            >
              <span className="i-fluent-edit-24-regular" role="img" aria-hidden="true" />
            </ActionIcon>
            <ActionIcon
              variant="filled"
              color="red"
              size="lg"
              radius="sm"
              onClick={() => {
                removeItem(item.id);
              }}
            >
              <span className="i-fluent-delete-24-regular" role="img" aria-hidden="true" />
            </ActionIcon>
          </div>
        </Table.Td>
      </Table.Tr>
    );
  });

  return (
    <Table miw={800} verticalSpacing="sm">
      <Table.Thead>
        <Table.Tr>
          <Table.Th className="w-16">
            <Checkbox
              onChange={toggleAll}
              checked={selection.length === data.length}
              indeterminate={selection.length > 0 && selection.length !== data.length}
            />
          </Table.Th>
          <Table.Th>名称</Table.Th>
          <Table.Th>最新章节</Table.Th>
          <Table.Th className="w-40">操作</Table.Th>
        </Table.Tr>
      </Table.Thead>
      <Table.Tbody>{rows}</Table.Tbody>
    </Table>
  );
}
