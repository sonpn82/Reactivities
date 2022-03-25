import React, {useCallback} from 'react'
import {useDropzone} from 'react-dropzone'
import { Header, Icon } from 'semantic-ui-react';

interface Props {
  setFiles: (files: any) => void;
}

export default function PhotoWidgetDropzone({setFiles}: Props) {

  // style for dropzone
  const dzStyle = {
    border: 'dashed 3px #eee',
    borderColor: '#eee',
    borderRadius: '5px',
    paddingTop: '30px',
    textAlign: 'center' as 'center',  // to avoid error warning in typescript about center
    height: 200
  }

  // additional style when dropzone is active
  const dzActive = {
    borderColor: 'green'
  }

  const onDrop = useCallback(acceptedFiles => {
    // Do something with the files
    setFiles(acceptedFiles.map((file: any) => Object.assign(file, {
      preview: URL.createObjectURL(file)
    })))
  }, [setFiles])
  const {getRootProps, getInputProps, isDragActive} = useDropzone({onDrop})

  return (
    // set style for the dropzone
    <div {...getRootProps()} style={isDragActive ? {...dzStyle, ...dzActive} : dzStyle}>
      <input {...getInputProps()} />
      <Icon name='upload' size='huge'/>
      <Header content='Drop image here' />
    </div>
  )
}